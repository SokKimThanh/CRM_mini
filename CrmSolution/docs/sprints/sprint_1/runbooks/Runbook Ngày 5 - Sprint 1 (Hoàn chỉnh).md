# RUNBOOK NGÀY 5 — SPRINT 1: ENUMS, HELPERS, THEME, LAYOUT & SERILOG

* **Sprint:** 1

* **File:** `docs/sprints/sprint_1/runbooks/day_5_enums_helpers_layout_serilog.md`

* **Target Framework:** .NET 8 / 9 / 10 | MudBlazor | Serilog

* **Chế độ thực thi:**

  * **Mode A (Fast-Track \~45 phút):** Chạy tuần tự các script PowerShell, đối soát bảng kiểm tra, commit Git.

  * **Mode B (Learn-Track \~120 phút):** Đọc kỹ các khối `[LEARN]` và thực hiện các bài test "Break-it" để hiểu sâu cơ chế hoạt động.

## 0. PRE-FLIGHT CHECK (KIỂM TRA TIỀN TRÌNH)

Chạy script sau trong PowerShell tại thư mục gốc của repository:

```
# 0.0 Tự động nhận diện Solution Root
$SolutionRoot = $PSScriptRoot
while (-not (Test-Path (Join-Path $SolutionRoot "*.sln")) -and $SolutionRoot -ne (Split-Path $SolutionRoot -Qualifier)) {
    $SolutionRoot = Split-Path $SolutionRoot -Parent
}
if (-not (Test-Path (Join-Path $SolutionRoot "*.sln"))) {
    $SolutionRoot = Get-Location
}
Set-Location $SolutionRoot
Write-Host ">> Working directory: $SolutionRoot" -ForegroundColor Cyan

# 0.1 Git checkpoint trước phiên làm việc
if (Test-Path ".git") {
    git add .
    git commit -m "chore: checkpoint before Day 5" --allow-empty
}

# 0.2 Kiểm tra trạng thái build của Day 4
dotnet build
if ($LASTEXITCODE -ne 0) { 
    throw "Build Day 4 thất bại. Hãy sửa toàn bộ lỗi của Day 4 trước khi tiếp tục!" 
}

# 0.3 Kiểm tra các file điều kiện tiên quyết
$requiredFiles = @(
    "src\Crm.Web\wwwroot\js\auth-interop.js",
    "src\Crm.Web\Components\Pages\Account\Login.razor",
    "src\Crm.Web\Components\Routes.razor"
)
foreach ($file in $requiredFiles) {
    if (-not (Test-Path $file)) { 
        throw "Thiếu file điều kiện tiên quyết: $file" 
    }
}

# 0.4 Kiểm tra Login.razor phải có @layout EmptyLayout
$loginContent = Get-Content "src\Crm.Web\Components\Pages\Account\Login.razor" -Raw
if ($loginContent -notmatch '@layout\s+EmptyLayout') {
    throw "Login.razor thiếu directive '@layout EmptyLayout' — sẽ gây hiện tượng redirect loop vô hạn!"
}

# 0.5 Tắt app đang chạy ngầm để tránh xung đột file/cổng mạng
Get-Process -Name "Crm.Web" -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
    try { $_.MainModule.FileName -like "*CrmSolution*" } catch { $false }
} | Stop-Process -Force -ErrorAction SilentlyContinue

# 0.6 Dọn dẹp cache build và log cũ
dotnet clean
Remove-Item "src\Crm.Web\logs\*.log" -Force -ErrorAction SilentlyContinue

Write-Host ">> Pre-flight OK: Môi trường hoàn toàn sẵn sàng." -ForegroundColor Green

```

## 1. MỤC TIÊU XÁC THỰC (VERIFICATION TARGETS)

| 

| **#** | **Hạng mục** | **Kỳ vọng kỹ thuật** | 
| 1 | **Enum files** | 5 file trong `src\Crm.Domain\Enums\` | 
| 2 | **Helper files** | 2 file trong `src\Crm.Business\Helpers\` | 
| 3 | **Theme** | `src\Crm.Web\Theme\CrmTheme.cs` cấu hình đầy đủ Light/Dark Palette | 
| 4 | **Layout** | `EmptyLayout`, `NavMenu`, `MainLayout` + `MainLayout.razor.css` | 
| 5 | **Serilog** | Ghi log vào file xoay vòng `src\Crm.Web\logs\crm-YYYYMMDD.log` | 
| 6 | **Audit CSS** | 0 inline style tĩnh trong toàn bộ cây thư mục `.razor` | 
| 7 | **Build** | 0 Warning, 0 Error (`dotnet build --no-incremental`) | 
| 8 | **Login Flow** | `admin@crm.local / Admin@2026` đăng nhập và chuyển hướng mượt | 
| 9 | **Dark Mode** | Nút toggle trên AppBar chuyển đổi trạng thái giao diện tức thì | 
| 10 | **Sidebar** | 6 menu items (1 Dashboard active, 5 module nghiệp vụ disabled) | 

## PHASE 1 — 5 DOMAIN ENUMS (12 PHÚT)

### 1.1 Khởi tạo file Enums

```
$enumDir = "src\Crm.Domain\Enums"
New-Item -ItemType Directory -Force -Path $enumDir | Out-Null
$enc = New-Object System.Text.UTF8Encoding $false

$enums = @{
    "CustomerHealth.cs" = @'
namespace Crm.Domain.Enums;

public enum CustomerHealth
{
    New = 0,
    Healthy = 1,
    NeedAttention = 2,
    AtRisk = 3,
    Dormant = 4,
    Churned = 5
}
'@

    "SalesTaskStatus.cs" = @'
namespace Crm.Domain.Enums;

public enum SalesTaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Skipped = 3,
    Cancelled = 4
}
'@

    "TaskOutcome.cs" = @'
namespace Crm.Domain.Enums;

public enum TaskOutcome
{
    CustomerWillBuy = 1,
    NeedQuote = 2,
    AgreeMeeting = 3,
    CustomerBusy = 4,
    NoAnswer = 5,
    AlreadyBought = 6,
    Complaint = 7,
    CareOnly = 8
}
'@

    "QuoteStatus.cs" = @'
namespace Crm.Domain.Enums;

public enum QuoteStatus
{
    Draft = 0,
    PendingApproval = 1,
    Sent = 2,
    Accepted = 3,
    Rejected = 4,
    Expired = 5
}
'@

    "InteractionType.cs" = @'
namespace Crm.Domain.Enums;

public enum InteractionType
{
    Call = 1,
    Meeting = 2,
    Email = 3,
    Zalo = 4,
    Visit = 5
}
'@
}

foreach ($name in $enums.Keys) {
    [System.IO.File]::WriteAllText((Join-Path $enumDir $name), $enums[$name], $enc)
    Write-Host "  + $name" -ForegroundColor Green
}

$count = (Get-ChildItem "$enumDir\*.cs").Count
if ($count -ne 5) { throw "Lỗi: Chỉ tìm thấy $count/5 enum files!" }
Write-Host ">> Phase 1 OK: Đã sinh đủ 5 Enums." -ForegroundColor Green

```

### 1.2 Bảng xử lý sai lệch (Deviation Guide)

| **Hiện tượng** | **Nguyên nhân** | **Biện pháp xử lý** | 
| `CS0104: TaskStatus ambiguous` | Đặt tên trùng với `System.Threading.Tasks.TaskStatus` trong BCL | Luôn đặt tiền tố `SalesTaskStatus` cho entity nghiệp vụ | 
| Lỗi hiển thị font chữ tiếng Việt | File lưu ở encoding ANSI | Luôn chỉ định `UTF8Encoding($false)` (UTF-8 No BOM) | 

> \[!TIP\] **\[LEARN\] — Mode B: Tại sao dùng `SalesTaskStatus` thay vì `TaskStatus`?** Thư viện cơ sở của .NET (Base Class Library) chứa `System.Threading.Tasks.TaskStatus`. Nếu đặt tên enum domain là `TaskStatus`, khi các service xử lý bất đồng bộ kết hợp entity nghiệp vụ, compiler sẽ báo lỗi mơ hồ kiểu tham chiếu `CS0104`.
>
> *Break-it test:* Đổi tên `SalesTaskStatus` thành `TaskStatus` rồi chạy `dotnet build` để quan sát lỗi, sau đó khôi phục bằng `git restore src/Crm.Domain/Enums/`.

## PHASE 2 — 2 UTILITY HELPERS (13 PHÚT)

### 2.1 Sinh mã nguồn Helpers

```
$helperDir = "src\Crm.Business\Helpers"
New-Item -ItemType Directory -Force -Path $helperDir | Out-Null
$enc = New-Object System.Text.UTF8Encoding $false

$currency = @'
using System.Globalization;

namespace Crm.Business.Helpers;

public static class CurrencyHelper
{
    private static readonly CultureInfo VietnamCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string Format(decimal amount)
        => amount.ToString("N0", VietnamCulture);

    public static string FormatShort(decimal amount)
    {
        var absAmount = Math.Abs(amount);
        var sign = amount < 0 ? "-" : "";

        if (absAmount >= 1_000_000_000m)
            return $"{sign}{(absAmount / 1_000_000_000m).ToString("0.##", VietnamCulture)} tỷ";
        if (absAmount >= 1_000_000m)
            return $"{sign}{(absAmount / 1_000_000m).ToString("0.##", VietnamCulture)} tr";
        if (absAmount >= 1_000m)
            return $"{sign}{(absAmount / 1_000m).ToString("0.##", VietnamCulture)}K";

        return amount.ToString("N0", VietnamCulture);
    }
}
'@

$date = @'
using System;

namespace Crm.Business.Helpers;

public static class DateHelper
{
    private static readonly TimeZoneInfo VietnamTz = ResolveVietnamTimeZone();

    private static TimeZoneInfo ResolveVietnamTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); } catch { }
        try { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); } catch { }
        return TimeZoneInfo.CreateCustomTimeZone("ICT", TimeSpan.FromHours(7), "Indochina Time", "ICT");
    }

    public static DateTime ToVietnamTime(DateTime utc)
    {
        var standardizedUtc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(standardizedUtc, VietnamTz);
    }

    public static DateTime ToUtc(DateTime vietnamTime)
    {
        var unspecified = DateTime.SpecifyKind(vietnamTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecified, VietnamTz);
    }

    public static int DaysBetween(DateTime from, DateTime to)
        => (to.Date - from.Date).Days;
}
'@

[System.IO.File]::WriteAllText((Join-Path $helperDir "CurrencyHelper.cs"), $currency, $enc)
[System.IO.File]::WriteAllText((Join-Path $helperDir "DateHelper.cs"), $date, $enc)

dotnet build src\Crm.Business\Crm.Business.csproj
if ($LASTEXITCODE -ne 0) { throw "Build Crm.Business thất bại!" }
Write-Host ">> Phase 2 OK: 2 Helpers biên dịch thành công." -ForegroundColor Green

```

### 2.2 Bảng xử lý sai lệch

| **Hiện tượng** | **Nguyên nhân** | **Biện pháp xử lý** | 
| `TimeZoneNotFoundException` trên Linux/Docker | Windows dùng tên Registry ID (`SE Asia Standard Time`), Linux dùng IANA ID (`Asia/Ho_Chi_Minh`) | Đã tích hợp hàm fallback 3 tầng trong `DateHelper.cs` | 
| `ArgumentException: DateTimeKind must be Utc` | Đối số truyền vào hàm chuyển đổi có `Kind == Local` hoặc `Unspecified` | Sử dụng `DateTime.SpecifyKind` chuẩn hóa trước khi gọi `ConvertTimeFromUtc` | 

> \[!TIP\] **\[LEARN\] — Mode B: Cơ chế Fallback múi giờ 3 tầng**
>
> * **Tầng 1 (Windows Host):** Tra cứu mã định danh Registry `SE Asia Standard Time`.
>
> * **Tầng 2 (Linux/Docker):** Tra cứu chuẩn IANA `Asia/Ho_Chi_Minh`.
>
> * **Tầng 3 (Alpine/Minimal Container):** Nếu cả hai thất bại, hệ thống tự động sinh ra một `CustomTimeZone` mang nhãn `ICT` với độ lệch cố định $+07:00$.

## PHASE 3 — THEME & MASTER LAYOUT (30 PHÚT)

### 3.1 Khởi tạo thư mục UI

```
New-Item -ItemType Directory -Force -Path "src\Crm.Web\Theme" | Out-Null
New-Item -ItemType Directory -Force -Path "src\Crm.Web\Components\Layout" | Out-Null
New-Item -ItemType Directory -Force -Path "src\Crm.Web\Components\Shared" | Out-Null
$enc = New-Object System.Text.UTF8Encoding $false

```

### 3.2 CrmTheme.cs

```
$theme = @'
using MudBlazor;

namespace Crm.Web.Theme;

public static class CrmTheme
{
    public static readonly MudTheme Current = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#667eea",
            Secondary = "#764ba2",
            AppbarBackground = "#667eea",
            Background = "#f8fafc",
            Surface = "#ffffff",
            DrawerBackground = "#ffffff",
            DrawerText = "#4a5568",
            AppbarText = "#ffffff"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#818cf8",
            Secondary = "#a78bfa",
            AppbarBackground = "#1f2937",
            Background = "#111827",
            Surface = "#1f2937",
            DrawerBackground = "#1f2937",
            DrawerText = "#e5e7eb",
            AppbarText = "#f9fafb"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "6px"
        }
    };
}
'@
[System.IO.File]::WriteAllText("src\Crm.Web\Theme\CrmTheme.cs", $theme, $enc)

```

### 3.3 MainLayout.razor.css

```
$css = @'
.crm-layout-wrapper ::deep .mud-appbar {
    border-bottom: 1px solid rgba(255, 255, 255, 0.12);
}

.crm-layout-wrapper ::deep .mud-drawer {
    border-right: 1px solid var(--mud-palette-lines-default);
}
'@
[System.IO.File]::WriteAllText("src\Crm.Web\Components\Layout\MainLayout.razor.css", $css, $enc)

```

### 3.4 EmptyLayout.razor

```
$empty = @'
@inherits LayoutComponentBase

<MudThemeProvider Theme="CrmTheme.Current" />
<MudDialogProvider />
<MudSnackbarProvider />

@Body
'@
[System.IO.File]::WriteAllText("src\Crm.Web\Components\Layout\EmptyLayout.razor", $empty, $enc)

```

### 3.5 NavMenu.razor

```
$nav = @'
<MudNavMenu>
    <MudNavLink Href="/" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Dashboard">
        Dashboard
    </MudNavLink>

    @* Module nghiệp vụ khóa tạm cho Sprint 1 tiếp theo *@
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.People">Khách hàng</MudNavLink>
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.TrendingUp">Cơ hội bán hàng</MudNavLink>
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.RequestQuote">Báo giá</MudNavLink>
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.CheckCircle">Kế hoạch hành động</MudNavLink>
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.Inventory2">Sản phẩm</MudNavLink>

    <MudDivider Class="my-2" />

    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.AdminPanelSettings">Quản trị</MudNavLink>
</MudNavMenu>
'@
[System.IO.File]::WriteAllText("src\Crm.Web\Components\Layout\NavMenu.razor", $nav, $enc)

```

### 3.6 MainLayout.razor

```
$main = @'
@inherits LayoutComponentBase

<MudThemeProvider Theme="CrmTheme.Current" @bind-IsDarkMode="_isDarkMode" />
<MudDialogProvider />
<MudSnackbarProvider />

<div class="crm-layout-wrapper">
    <MudLayout>
        <MudAppBar Elevation="1" Color="Color.Primary">
            <MudIconButton Icon="@Icons.Material.Filled.Menu"
                           Color="Color.Inherit"
                           Edge="Edge.Start"
                           OnClick="@ToggleDrawer" />

            <MudText Typo="Typo.h6" Class="ml-2 font-weight-bold">CRM System</MudText>
            <MudSpacer />

            <MudIconButton Icon="@(_isDarkMode ? Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode)"
                           Color="Color.Inherit"
                           OnClick="@ToggleTheme"
                           Title="Chuyển chế độ giao diện" />

            <MudText Class="mx-3">@_userName</MudText>

            <MudIconButton Icon="@Icons.Material.Filled.Logout"
                           Color="Color.Inherit"
                           OnClick="@HandleLogout"
                           Title="Đăng xuất" />
        </MudAppBar>

        <MudDrawer @bind-Open="_drawerOpen" Elevation="0" ClipMode="DrawerClipMode.Always">
            <NavMenu />
        </MudDrawer>

        <MudMainContent Class="pa-4">
            @Body
        </MudMainContent>
    </MudLayout>
</div>

@code {
    [CascadingParameter]
    private Task<AuthenticationState>? AuthState { get; set; }

    [Inject] private IJSRuntime JS { get; set; } = default!;

    private bool _drawerOpen = true;
    private bool _isDarkMode = false;
    private string _userName = "";

    protected override async Task OnInitializedAsync()
    {
        if (AuthState is not null)
        {
            var state = await AuthState;
            _userName = state.User.Identity?.Name ?? "Người dùng";
        }
    }

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;
    private void ToggleTheme() => _isDarkMode = !_isDarkMode;

    private async Task HandleLogout()
    {
        await JS.InvokeVoidAsync("crmAuth.logout");
    }
}
'@
[System.IO.File]::WriteAllText("src\Crm.Web\Components\Layout\MainLayout.razor", $main, $enc)

```

### 3.7 Cập nhật `_Imports.razor` (Safe Append)

```
$importsPath = "src\Crm.Web\Components\_Imports.razor"
$content = Get-Content -Path$importsPath -Raw

$requiredImports = @(
    '@using Microsoft.AspNetCore.Components.Authorization',
    '@using Microsoft.AspNetCore.Components.Routing',
    '@using Microsoft.JSInterop',
    '@using MudBlazor',
    '@using Crm.Web.Theme',
    '@using Crm.Web.Components.Layout',
    '@using Crm.Web.Components.Shared'
)

foreach ($line in$requiredImports) {
    if ($content -notmatch [regex]::Escape($line)) {$content += "`r`n$line"
        Write-Host "  + $line" -ForegroundColor Green
    }
}

[System.IO.File]::WriteAllText($importsPath,$content, (New-Object System.Text.UTF8Encoding $false))

```

### 3.8 Kiểm tra Routes.razor & Build Phase 3

```
$routesPath = "src\Crm.Web\Components\Routes.razor"
$routes = Get-Content $routesPath -Raw

if ($routes -notmatch 'typeof\((Layout\.)?MainLayout\)') {
    throw "Routes.razor chưa cấu hình DefaultLayout = typeof(MainLayout)!"
}
if ($routes -notmatch 'RedirectToLogin') {
    Write-Host "  [!] Cảnh báo: Routes.razor chưa có <RedirectToLogin /> cho NotAuthorized." -ForegroundColor Yellow
} else {
    Write-Host "  Routes.razor hợp lệ." -ForegroundColor Green
}

dotnet build --no-incremental
if ($LASTEXITCODE -ne 0) { throw "Build Phase 3 thất bại!" }
Write-Host ">> Phase 3 OK: Master Layout & Theme hoàn tất." -ForegroundColor Green

```

### 3.9 Bảng xử lý sai lệch

| **Hiện tượng** | **Nguyên nhân** | **Biện pháp xử lý** | 
| CSS `::deep` không ăn hiệu ứng | Thiếu phần tử HTML gốc bọc ngoài component | Bắt buộc phải có `<div class="crm-layout-wrapper">` | 
| `CrmTheme not found` | Chưa nạp namespace vào Razor | Kiểm tra `@using Crm.Web.Theme` trong `_Imports.razor` | 
| Dark mode không đổi màu | Binding `@bind-IsDarkMode` bị mất kết nối | Kiểm tra cú pháp `@bind-IsDarkMode="_isDarkMode"` tại `<MudThemeProvider>` | 
| Xuất hiện thanh cuộn kép | CSS ép cứng `height: 100vh` hoặc `calc(...)` | Để `MudLayout` tự quản trị chiều cao tự nhiên | 

> \[!TIP\] **\[LEARN\] — Mode B: Tại sao bắt buộc dùng `.crm-layout-wrapper`?** Blazor Scoped CSS hoạt động bằng cách sinh mã hash độc nhất (ví dụ `b-x89fjs`) và gán vào các thẻ HTML gốc. `<MudLayout>` và `<MudAppBar>` là các C# Razor Components, không nhận trực tiếp mã hash này. Thẻ bao ngoài `<div class="crm-layout-wrapper">` đóng vai trò là phần tử HTML chuẩn duy nhất để trình biên dịch dựng selector: `.crm-layout-wrapper[b-x89fjs] ::deep .mud-appbar`.

## PHASE 4 — SERILOG INTEGRATION (25 PHÚT)

### 4.1 Cài đặt NuGet Packages

```
Set-Location "src\Crm.Web"

$csproj = Get-Content "Crm.Web.csproj" -Raw
if ($csproj -notmatch 'Serilog\.AspNetCore') {
    dotnet add package Serilog.AspNetCore --version 8.0.3
}
if ($csproj -notmatch 'Serilog\.Sinks\.File') {
    dotnet add package Serilog.Sinks.File --version 5.0.0
}

Set-Location $SolutionRoot

```

### 4.2 Cấu hình `src\Crm.Web\appsettings.json`

Đảm bảo file `appsettings.json` có cấu hình Serilog (giữ nguyên cấu hình `ConnectionStrings` hiện tại):

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=crm_db;Username=crm_user;Password=sa"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/crm-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
  },
  "AllowedHosts": "*"
}

```

### 4.3 Cập nhật `src\Crm.Web\Program.cs` (3 vị trí cố định)

> \[!WARNING\] Không ghi đè toàn bộ `Program.cs`. Chỉ chèn mã vào đúng 3 vị trí sau để giữ nguyên cấu hình Identity & DbContext của Day 3/Day 4:

#### Vị trí 1: Đầu file (sau các using)

```
using System;
using System.IO;
using Serilog;

```

#### Vị trí 2: Trước dòng `var builder = WebApplication.CreateBuilder(args);`

```
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

```

#### Vị trí 3: Cuối file (thay thế lệnh `app.Run();`)

```
try
{
    Log.Information("Starting CRM System");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

```

### 4.4 Khởi tạo thư mục Logs và cập nhật `.gitignore`

```
New-Item -ItemType Directory -Force -Path "src\Crm.Web\logs" | Out-Null

$gitignore = ".gitignore"
if (-not (Test-Path $gitignore)) { New-Item -ItemType File $gitignore | Out-Null }
if (-not (Select-String -Path $gitignore -Pattern "src/Crm.Web/logs/" -Quiet)) {
    Add-Content $gitignore "`nsrc/Crm.Web/logs/"
    Write-Host "  Đã thêm src/Crm.Web/logs/ vào .gitignore" -ForegroundColor Green
}

dotnet build --no-incremental
if ($LASTEXITCODE -ne 0) { throw "Build Phase 4 thất bại với Serilog!" }
Write-Host ">> Phase 4 OK: Serilog tích hợp thành công." -ForegroundColor Green

```

## PHASE 5 — AUDIT, TEST VÀ COMMIT (20 PHÚT)

### 5.1 Quét sạch Inline Style tĩnh (Zero-Inline Audit)

```
$violations = @()
Get-ChildItem -Path "src\Crm.Web" -Filter "*.razor" -Recurse | ForEach-Object {
    $file = $_
    $lineNum = 0
    Get-Content $file.FullName | ForEach-Object {
        $lineNum++
        if ($_ -match 'style\s*=\s*["''][^"'']*["'']') {
            if ($_ -notmatch '@\(' -and $_ -notmatch '@\$') {
                $violations += "$($file.Name):$lineNum -> $($_.Trim())"
            }
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Host "Cảnh báo: Phát hiện inline style tĩnh vi phạm quy chuẩn:" -ForegroundColor Red
    $violations | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    throw "Zero-Inline Audit FAIL: Có $($violations.Count) vị trí inline style cần chuyển sang CSS class!"
} else {
    Write-Host ">> Zero-Inline Audit PASS: 100% không chứa inline style tĩnh." -ForegroundColor Green
}

```

### 5.2 Khởi chạy ứng dụng

```
Set-Location "src\Crm.Web"
dotnet run

```

### 5.3 Bảng kiểm tra E2E thủ công (Manual E2E Checklist)

| **#** | **Thao tác kiểm tra** | **Kết quả kỳ vọng** | **Trạng thái** | 
| 1 | Mở terminal khởi động | Xuất hiện log `Starting CRM System` theo định dạng Serilog | \[ \] | 
| 2 | Truy cập `https://localhost:7059` | Tự động chuyển hướng về trang đăng nhập `/login` | \[ \] | 
| 3 | Đăng nhập tài khoản admin | Dùng `admin@crm.local / Admin@2026`, chuyển về `/` và hiện tên user | \[ \] | 
| 4 | Bấm nút Dark Mode toggle | Giao diện đổi mượt mà giữa Dark và Light Palette | \[ \] | 
| 5 | Kiểm tra thanh điều hướng (Sidebar) | Đủ 6 menu: Dashboard (active), 5 chức năng còn lại ở trạng thái disabled | \[ \] | 
| 6 | Bấm nút Drawer Menu trên AppBar | Thanh Sidebar đóng mở linh hoạt không bị giật layout | \[ \] | 
| 7 | Kiểm tra thư mục `logs/` | File `crm-YYYYMMDD.log` xuất hiện và ghi nhận các lượt request | \[ \] | 

### 5.4 Xác nhận Log hoạt động

Mở một cửa sổ PowerShell mới và kiểm tra:

```
Get-Content "src\Crm.Web\logs\crm-*.log" -Tail 15

```

### 5.5 Thực hiện Git Commit

Sau khi nhấn `Ctrl + C` để dừng ứng dụng an toàn:

```
Set-Location $SolutionRoot

git add .
git commit -m "feat(day5): enums, helpers, theme, master layout, serilog

- 5 enums: CustomerHealth, SalesTaskStatus, TaskOutcome, QuoteStatus, InteractionType
- 2 helpers: CurrencyHelper (VND), DateHelper (UTC <-> GMT+7, 3-tier fallback)
- Centralized CrmTheme.cs with Light/Dark palette
- Master layout: Scoped CSS (::deep), Dark Mode toggle, Drawer
- NavMenu: 6 items (1 active, 5 disabled for Day 6+)
- Serilog: bootstrap logger + console + rolling file logs/
- Zero-Inline Audit passed"

```

### 5.6 Ghi nhận Daily Log (`docs/sprints/sprint_1/notes/daily.md`)

```
# Daily Log — Day 5 / Sprint 1

## Done
- [x] 5 enums nghiệp vụ trong `Crm.Domain/Enums`
- [x] 2 utility helpers trong `Crm.Business/Helpers`
- [x] Hệ thống Theme tập trung `CrmTheme.cs`
- [x] Master layout với Dark Mode toggle và Scoped CSS
- [x] Serilog: Console + Rolling Daily File
- [x] Zero-Inline Audit PASS (0 vi phạm)
- [x] Hoàn tất Git commit an toàn

## Metrics
- Enums: 5 | Helpers: 2 | Layout: 3 | Theme: 1
- Build: 0 Warning, 0 Error
- Log path: `src/Crm.Web/logs/crm-YYYYMMDD.log`

## Next Steps (Day 6)
- CustomerRepository & CustomerService
- Bộ Unit Test cho CustomerService
- Giao diện danh sách `/customers` với MudTable

```

## 2. BẢNG TỔNG HỢP XỬ LÝ SỰ CỐ (TROUBLESHOOTING MATRIX)

| **Hiện tượng** | **Nguyên nhân gốc** | **Cách khắc phục triệt để** | 
| `CS0104: TaskStatus ambiguous` | Xung đột với namespace `System.Threading.Tasks` | Dùng tên `SalesTaskStatus` cho toàn bộ tầng domain | 
| Scoped CSS `::deep` không có tác dụng | Thiếu wrapper HTML bọc ngoài | Đảm bảo `<div class="crm-layout-wrapper">` bọc quanh `<MudLayout>` | 
| Nút Dark Mode không đổi màu | Thiếu two-way binding | Kiểm tra `@bind-IsDarkMode="_isDarkMode"` tại `<MudThemeProvider>` | 
| Giao diện bị thanh cuộn kép | Cố định chiều cao `100vh` thủ công | Gỡ bỏ `calc(100vh - ...)` để MudBlazor tự tính chiều cao | 
| `TimeZoneNotFoundException` trên Linux | Thiếu mã ID của Windows Registry | Sử dụng fallback sang IANA `Asia/Ho_Chi_Minh` hoặc Custom Offset | 
| `CrmTheme not found` | Chưa import namespace vào Razor | Thêm `@using Crm.Web.Theme` vào `_Imports.razor` | 
| `NavLinkMatch not found` | Thiếu namespace Routing | Thêm `@using Microsoft.AspNetCore.Components.Routing` vào `_Imports.razor` | 
| File log không tự tạo | Chạy sai thư mục làm việc | Chạy `dotnet run` từ thư mục `src\Crm.Web` | 
| Bấm Logout không phản hồi | Script JS Interop chưa tải | Mở tab Console (F12) kiểm tra file `auth-interop.js` | 
| Bị redirect loop liên tục | `Login.razor` kế thừa `MainLayout` | Bổ sung directive `@layout EmptyLayout` vào đầu file `Login.razor` | 

## 3. CHECKLIST KIỂM THỬ CUỐI PHIÊN (FINAL AUDIT CHECKLIST)

* \[ \] 5 file enum tồn tại tại `src\Crm.Domain\Enums\`

* \[ \] 2 file helper tồn tại tại `src\Crm.Business\Helpers\`

* \[ \] `CrmTheme.cs` tồn tại tại `src\Crm.Web\Theme\`

* \[ \] `MainLayout.razor.css` tồn tại và chứa selector `.crm-layout-wrapper ::deep`

* \[ \] `MainLayout.razor` được bao bởi thẻ `<div class="crm-layout-wrapper">`

* \[ \] `_Imports.razor` có đầy đủ các directives `@using` cần thiết

* \[ \] `Routes.razor` trỏ đúng `DefaultLayout = typeof(MainLayout)`

* \[ \] `Login.razor` có directive `@layout EmptyLayout`

* \[ \] `Program.cs` tích hợp Serilog đầy đủ 3 vị trí

* \[ \] `appsettings.json` chứa section `"Serilog"` hợp lệ

* \[ \] Thư mục `src\Crm.Web\logs\` tồn tại và nằm trong `.gitignore`

* \[ \] Zero-Inline Audit PASS (không có vi phạm style tĩnh)

* \[ \] `dotnet build --no-incremental` đạt 0 Warning, 0 Error

* \[ \] Đăng nhập thành công với tài khoản quản trị

* \[ \] Tính năng Dark Mode và Sidebar Drawer hoạt động chuẩn xác

* \[ \] File log trong ngày được tạo và ghi nhận đầy đủ luồng thực thi

* \[ \] Git commit thành công với message chuẩn conventional commits

* \[ \] Hoàn tất cập nhật daily log trong `daily.md`