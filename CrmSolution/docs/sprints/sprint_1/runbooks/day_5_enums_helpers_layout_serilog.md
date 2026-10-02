# RUNBOOK NGÀY 5 — SPRINT 1: ENUMS, HELPERS, MASTER LAYOUT & SERILOG

**Sprint**: 1
**Thời lượng dự kiến**: 120 phút (Bao gồm 25 phút buffer)
**Đường dẫn lưu trữ**: `docs/sprints/sprint_1/runbooks/day_5_enums_helpers_layout_serilog.md`
**Môi trường**: Local Development (.NET 10 / C# 13 / MudBlazor / PostgreSQL)

---

## BỐI CẢNH DỰ ÁN (PROJECT CONTEXT)
* **Tiến trình**: Chúng ta đang ở Ngày 5 của Sprint 1.
* **Đã hoàn thành (Day 4)**: Cấu trúc cơ sở dữ liệu đã được khởi tạo (Database-First), các Entity đã được Scaffold bằng EF Core.
* **Mục tiêu hôm nay**: Xây dựng nền tảng khung giao diện (Master Layout với MudBlazor), các hằng số nghiệp vụ (Enums), công cụ hỗ trợ (Helpers) và hệ thống ghi log chuẩn (Serilog). Đây là bước đệm bắt buộc để chuyển sang thiết kế các trang nghiệp vụ (Khách hàng, Báo giá) ở các ngày tiếp theo.

---

## 1. MỤC TIÊU & CHỈ TIÊU NGHIỆM THU (VERIFICATION TARGETS)

### 1.1 Bảng Mục Tiêu (Objectives)
| Mục tiêu | Mô tả chi tiết |
| :--- | :--- |
| **Domain Enums** | Xây dựng 5 Enum nghiệp vụ cốt lõi tại `Crm.Domain\Enums\`. |
| **Business Helpers** | Xây dựng 2 Utility Helper đa nền tảng tại `Crm.Business\Helpers\`. |
| **MudBlazor Layouts** | Hoàn thiện 3 file hệ thống Layout: `EmptyLayout`, `NavMenu`, `MainLayout`. |
| **Serilog Sink** | Tích hợp Serilog ghi log có cấu trúc ra Console và File xoay vòng theo ngày. |

### 1.2 Bảng Chỉ Tiêu Nghiệm Thu (Verification Targets)
| Hạng mục | Tiêu chí đạt chuẩn | Phương pháp kiểm tra |
| :--- | :--- | :--- |
| **Domain Enums** | 5 file `.cs` tạo đúng thư mục `Crm.Domain.Enums` | PowerShell file count |
| **Business Helpers** | 2 file `CurrencyHelper.cs` và `DateHelper.cs` | PowerShell check & `dotnet build` |
| **MudBlazor Layouts** | 3 file UI hoạt động, hiển thị đúng Sidebar 6 menu items | Mở trình duyệt kiểm tra trực quan |
| **Serilog Sink** | Sinh file log định dạng `crm-YYYYMMDD.log` | Kiểm tra thư mục `logs/` |
| **Độ ổn định** | 0 Warning, 0 Error toàn Solution | `dotnet build --no-incremental` |
| **Auth Flow** | Login Admin thành công và redirect đúng MainLayout | Manual Browser E2E verify |

---

## 2. ĐIỀU KIỆN TIÊN QUYẾT (PRE-FLIGHT CHECKS)

> 🕒 **Time Budget:** 5 Phút

**Bắt buộc**: Đảm bảo phiên làm việc Day 4 đã hoàn tất.
Mở PowerShell tại thư mục gốc của dự án (`CrmSolution`):

```powershell
# Di chuyển vào gốc dự án
cd $(Get-Location).Path

# 1. Kiểm tra trạng thái build hiện tại
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Error "[FAIL] Dự án hiện tại không build được. Yêu cầu quay lại Day 4!"
    # Stop Execution
}

# 2. Kiểm tra JS interop cho auth (Kết quả từ Day 3)
$jsInteropPath = "src/Crm.Web/wwwroot/js/auth-interop.js"
if (-not (Test-Path $jsInteropPath)) {
    Write-Error "[FAIL] Không tìm thấy: $jsInteropPath. Phải có file này để Auth hoạt động!"
    # Stop Execution
}

Write-Host "[OK] Điều kiện tiên quyết: ĐẠT CHUẨN." -ForegroundColor Green
```

*Output kỳ vọng:*
```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
[OK] Điều kiện tiên quyết: ĐẠT CHUẨN.
```

---

## 3. CÁC BƯỚC THỰC THI CHI TIẾT (STEP-BY-STEP FLOW)

### PHASE 1 — TẠO 5 ENUMS NGHIỆP VỤ
> 🕒 **Time Budget:** 12 Phút

**1. Setup & Execution (Thực thi):**
Chạy script PowerShell sau từ gốc dự án để tạo thư mục và sinh 5 file Enum. Mọi file được tạo với định dạng UTF-8 (No BOM).
*(Lưu ý Kiến trúc: Đặt tên `SalesTaskStatus` thay vì `TaskStatus` để tránh lỗi CS0104 với Base Class của .NET).*

```powershell
$enumDir = "src/Crm.Domain/Enums"
New-Item -ItemType Directory -Force -Path $enumDir | Out-Null

$utf8NoBom = New-Object System.Text.UTF8Encoding $false

$enumDefinitions = @{
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

foreach ($fileName in $enumDefinitions.Keys) {
    $fullPath = Join-Path $enumDir $fileName
    [System.IO.File]::WriteAllText($fullPath, $enumDefinitions[$fileName], $utf8NoBom)
    Write-Host "[OK] Tạo file: $fileName" -ForegroundColor Green
}
```

**2. Verification (Xác thực):**
```powershell
$count = (Get-ChildItem -Path "src/Crm.Domain/Enums/*.cs").Count
if ($count -eq 5) { Write-Host "[OK] Phase 1 Checkpoint: Đã tạo 5 files." -ForegroundColor Green }
```

---

### PHASE 2 — TẠO 2 UTILITY HELPERS
> 🕒 **Time Budget:** 13 Phút

**1. Setup & Execution (Thực thi):**
Tạo hàm định dạng tiền tệ VNĐ và xử lý múi giờ UTC <-> GMT+7 (Cross-platform cho cả Windows/Linux).

```powershell
$helperDir = "src/Crm.Business/Helpers"
New-Item -ItemType Directory -Force -Path $helperDir | Out-Null

$currencyHelperCode = @'
using System.Globalization;

namespace Crm.Business.Helpers;

public static class CurrencyHelper
{
    private static readonly CultureInfo VietnamCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string Format(decimal amount) => amount.ToString("N0", VietnamCulture);

    public static string FormatShort(decimal amount)
    {
        if (amount >= 1_000_000_000) return $"{amount / 1_000_000_000:0.##} tỷ";
        if (amount >= 1_000_000) return $"{amount / 1_000_000:0.##} tr";
        if (amount >= 1_000) return $"{amount / 1_000:0.##}K";
        return amount.ToString("N0", VietnamCulture);
    }
}
'@

$dateHelperCode = @'
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

    public static DateTime ToVietnamTime(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(utc, VietnamTz);

    public static DateTime ToUtc(DateTime vietnam) => TimeZoneInfo.ConvertTimeToUtc(vietnam, VietnamTz);

    public static int DaysBetween(DateTime from, DateTime to) => (to.Date - from.Date).Days;
}
'@

[System.IO.File]::WriteAllText((Join-Path $helperDir "CurrencyHelper.cs"), $currencyHelperCode, $utf8NoBom)
[System.IO.File]::WriteAllText((Join-Path $helperDir "DateHelper.cs"), $dateHelperCode, $utf8NoBom)
Write-Host "  [OK] Tạo file: CurrencyHelper.cs" -ForegroundColor Green
Write-Host "  [OK] Tạo file: DateHelper.cs" -ForegroundColor Green
```

**2. Verification (Xác thực):**
```powershell
dotnet build src/Crm.Business/Crm.Business.csproj
```
*Output kỳ vọng:* `Build succeeded. 0 Warning(s) 0 Error(s)`

---

### PHASE 3 — HOÀN THIỆN MUDLAZOR MASTER LAYOUT
> 🕒 **Time Budget:** 25 Phút

**1. Setup & Execution (Thực thi):**
Tạo cấu trúc UI Components.

```powershell
New-Item -ItemType Directory -Force -Path "src/Crm.Web/Components/Layout" | Out-Null
New-Item -ItemType Directory -Force -Path "src/Crm.Web/Components/Shared" | Out-Null

$emptyLayoutContent = @'
@inherits LayoutComponentBase

<MudThemeProvider />
<MudDialogProvider />
<MudSnackbarProvider />

@Body
'@
[System.IO.File]::WriteAllText("src/Crm.Web/Components/Layout/EmptyLayout.razor", $emptyLayoutContent, $utf8NoBom)

$navMenuContent = @'
<MudNavMenu>
    <MudNavLink Href="/" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Dashboard">
        Dashboard
    </MudNavLink>

    @* Routes nghiệp vụ: Tạm khóa bằng Disabled=true cho đến Day 6+ *@
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.People">
        Khách hàng
    </MudNavLink>
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.TrendingUp">
        Cơ hội bán hàng
    </MudNavLink>
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.RequestQuote">
        Báo giá
    </MudNavLink>
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.CheckCircle">
        Kế hoạch hành động
    </MudNavLink>
    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.Inventory2">
        Sản phẩm
    </MudNavLink>

    <MudDivider Class="my-2" />

    <MudNavLink Href="#" Disabled="true" Icon="@Icons.Material.Filled.AdminPanelSettings">
        Quản trị
    </MudNavLink>
</MudNavMenu>
'@
[System.IO.File]::WriteAllText("src/Crm.Web/Components/Layout/NavMenu.razor", $navMenuContent, $utf8NoBom)

$mainLayoutContent = @'
@inherits LayoutComponentBase

<MudThemeProvider Theme="@_theme" />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudAppBar Elevation="1" Color="Color.Primary">
        <MudIconButton Icon="@Icons.Material.Filled.Menu"
                       Color="Color.Inherit"
                       Edge="Edge.Start"
                       OnClick="@ToggleDrawer" />
        <MudText Typo="Typo.h6" Class="ml-2">CRM System</MudText>
        <MudSpacer />
        <MudText Class="mr-4">@_userName</MudText>
        <MudIconButton Icon="@Icons.Material.Filled.Logout"
                       Color="Color.Inherit"
                       OnClick="@HandleLogout" />
    </MudAppBar>

    <MudDrawer @bind-Open="_drawerOpen" Elevation="2" ClipMode="DrawerClipMode.Always">
        <NavMenu />
    </MudDrawer>

    <MudMainContent Class="pa-4">
        @Body
    </MudMainContent>
</MudLayout>

@code {
    [CascadingParameter]
    private Task<AuthenticationState>? AuthState { get; set; }

    [Inject] private IJSRuntime JS { get; set; } = default!;

    private bool _drawerOpen = true;
    private string _userName = "";

    private readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#667eea",
            Secondary = "#764ba2",
            AppbarBackground = "#667eea"
        }
    };

    protected override async Task OnInitializedAsync()
    {
        if (AuthState is not null)
        {
            var state = await AuthState;
            _userName = state.User.Identity?.Name ?? "Người dùng";
        }
    }

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;

    private async Task HandleLogout()
    {
        await JS.InvokeVoidAsync("crmAuth.logout");
    }
}
'@
[System.IO.File]::WriteAllText("src/Crm.Web/Components/Layout/MainLayout.razor", $mainLayoutContent, $utf8NoBom)
```

**2. Cập nhật `_Imports.razor` và `Routes.razor`:**
Mở tệp `src/Crm.Web/Components/_Imports.razor` và đảm bảo các namespace sau đã được khai báo:
```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.JSInterop
@using MudBlazor
@using Crm.Web
@using Crm.Web.Components
@using Crm.Web.Components.Layout
@using Crm.Web.Components.Shared
```

Mở `src/Crm.Web/Components/Routes.razor` và kiểm tra `DefaultLayout`:
```razor
<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(Layout.MainLayout)">
            <NotAuthorized>
                <RedirectToLogin />
            </NotAuthorized>
        </AuthorizeRouteView>
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
</Router>
```

**3. Verification (Xác thực):**
```powershell
dotnet build
```
*Output kỳ vọng:* `Build succeeded. 0 Warning(s) 0 Error(s)`

---

### PHASE 4 — TÍCH HỢP SERILOG STRUCTURED LOGGING
> 🕒 **Time Budget:** 25 Phút

**1. Setup & Execution (Cài Package & Ghi cấu hình):**

```powershell
cd src/Crm.Web
dotnet add package Serilog.AspNetCore --version 8.0.3
dotnet add package Serilog.Sinks.File --version 5.0.0
cd ../..
```

**2. Cập nhật cấu hình Serilog trong `src/Crm.Web/appsettings.json`:**
```json
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
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**3. Chỉnh sửa `src/Crm.Web/Program.cs`:**
```csharp
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog; // [VỊ TRÍ 1]: Thêm Serilog namespace

// [VỊ TRÍ 2]: Khởi tạo Bootstrap Logger TRƯỚC KHI tạo builder
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting CRM System bootstrap...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(); // Tích hợp Serilog thay thế logging mặc định

    // ... (Giữ nguyên toàn bộ cấu hình Services cũ: DbContext, Identity, Blazor, MudBlazor ...)

    var app = builder.Build();

    // ... (Giữ nguyên toàn bộ cấu hình Middleware pipeline cũ ...)

    // [VỊ TRÍ 3]: Bọc app.Run trong khối try-catch-finally chuẩn Serilog
    Log.Information("CRM System ready. Running application pipeline.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly during startup or runtime");
}
finally
{
    Log.Information("CRM System shutting down. Flushing log sinks...");
    Log.CloseAndFlush();
}
```

**4. Khởi tạo thư mục log và .gitignore:**
```powershell
New-Item -ItemType Directory -Force -Path "src/Crm.Web/logs" | Out-Null

$gitignorePath = "src/Crm.Web/.gitignore"
if (-not (Test-Path $gitignorePath)) {
    New-Item -ItemType File -Path $gitignorePath | Out-Null
}
if (-not (Select-String -Path $gitignorePath -Pattern "^logs/" -Quiet)) {
    Add-Content -Path $gitignorePath -Value "`nlogs/"
    Write-Host "  [OK] Đã cấu hình bỏ qua thư mục logs/ trong Git." -ForegroundColor Green
}
```

**5. Verification (Xác thực):**
```powershell
dotnet build
```
*Output kỳ vọng:* `Build succeeded. 0 Warning(s) 0 Error(s)`

---

## 4. BẢNG KIỂM TRA TỔNG KẾT & E2E (FINAL AUDIT)

> 🕒 **Time Budget:** 10 Phút

### 4.1. Khởi chạy ứng dụng
```powershell
cd src/Crm.Web
dotnet run
```

### 4.2. E2E Audit Checklist
* [ ] Ứng dụng khởi động không văng lỗi, console hiển thị dạng log của Serilog (`[INF] Starting CRM System...`).
* [ ] Truy cập `https://localhost:<port>` bị redirect về trang Đăng nhập (`/login`).
* [ ] Đăng nhập thành công, Layout mới hiển thị MudAppBar (Header) và MudDrawer (Sidebar).
* [ ] Tên người dùng hiển thị đúng trên góc phải AppBar.
* [ ] Thanh Sidebar có 6 menu (1 Dashboard + 5 mục nghiệp vụ tạm Disable).
* [ ] Kiểm tra thư mục `src/Crm.Web/logs/` xuất hiện file `crm-YYYYMMDD.log`.

---

## 5. CẬP NHẬT DAILY LOG VÀ GIT COMMIT

### 5.1 Commit toàn bộ thay đổi
Mở Terminal, chạy lệnh:

```powershell
cd $(Get-Location).Path
git add .
git commit -m "feat(day5): add enums, helpers, master layout, serilog

- 5 enums: CustomerHealth, SalesTaskStatus, TaskOutcome, QuoteStatus, InteractionType
- 2 helpers: CurrencyHelper, DateHelper
- Master layout: MudAppBar + MudDrawer + NavMenu
- Serilog: console + rolling daily file logs/"
```

### 5.2 Cập nhật Daily Log
Ghi lại kết quả vào `docs/sprints/sprint_1/notes/daily.md`:

```markdown
# Daily Log — Day 5 / Sprint 1

## Done
- [x] 5 enums nghiệp vụ trong Crm.Domain/Enums
- [x] 2 utility helpers trong Crm.Business/Helpers
- [x] Hệ thống MudBlazor Master layout: MainLayout + NavMenu + EmptyLayout
- [x] Cấu hình Serilog structured logging
- [x] Xác thực Authentication Flow & Sidebar State
- [x] Git Commit & cập nhật tài liệu

## Metrics
- Enums: 5
- Helpers: 2
- Layout files: 3
- Build Status: 0 Warning, 0 Error
- Log Output: src/Crm.Web/logs/crm-YYYYMMDD.log
```

---

## 6. QUẢN TRỊ RỦI RO (TROUBLESHOOTING & ROLLBACK)

### 6.1 Kế hoạch Hoàn tác (Rollback Plan)
Nếu hệ thống gặp lỗi nghiêm trọng (không thể build, crash App), chạy ngay kịch bản sau để khôi phục:

```powershell
# Xóa thư mục Enums và Helpers vừa tạo
Remove-Item -Recurse -Force "src/Crm.Domain/Enums"
Remove-Item -Recurse -Force "src/Crm.Business/Helpers"

# Gỡ bỏ Serilog
cd src/Crm.Web
dotnet remove package Serilog.AspNetCore
dotnet remove package Serilog.Sinks.File

# Rollback cấu hình Git
cd ../..
git checkout src/Crm.Web/Program.cs
git checkout src/Crm.Web/appsettings.json
git checkout src/Crm.Web/Components/Routes.razor
git checkout src/Crm.Web/Components/_Imports.razor
Remove-Item -Recurse -Force "src/Crm.Web/Components/Layout/EmptyLayout.razor"
Remove-Item -Recurse -Force "src/Crm.Web/Components/Layout/NavMenu.razor"
Remove-Item -Recurse -Force "src/Crm.Web/Components/Layout/MainLayout.razor"
```

### 6.2 Root Cause Analysis (Bắt bệnh RCA)

| Triệu chứng (Symptom) | Nguyên nhân gốc (Root Cause) | Giải pháp (Solution) | Xác thực (Verification) |
| :--- | :--- | :--- | :--- |
| **Lỗi `CS0104: TaskStatus is an ambiguous reference`** | Đặt tên Enum trùng với class `TaskStatus` của .NET BCL. | Đổi tên enum thành `SalesTaskStatus`. | Build lại project Crm.Domain không lỗi. |
| **Lỗi `CS0246: The type NavLinkMatch could not be found`** | File `NavMenu.razor` chưa nhận biết được namespace Routing. | Thêm `@using Microsoft.AspNetCore.Components.Routing` vào `_Imports.razor`. | IDE không gạch đỏ `NavLinkMatch.All`. |
| **Không hiển thị Layout (màn hình trắng/dị dạng)** | `Routes.razor` chưa chỉnh `DefaultLayout` trỏ vào Component mới. | Trỏ lại thuộc tính `DefaultLayout="@typeof(Layout.MainLayout)"` trong `<AuthorizeRouteView>`. | F5 lại trình duyệt, Sidebar hiện ra. |
| **Không sinh ra file Log trong thư mục** | Serilog chưa khởi tạo đúng, hoặc đường dẫn `logs/` trong config sai (hoặc thiếu quyền). | Kiểm tra lại `appsettings.json` đoạn `path: "logs/crm-.log"`. Chạy app 1 lần. | Lệnh `Test-Path "src/Crm.Web/logs/"` trả về `True`. |
| **TimeZoneNotFoundException trên máy chủ Linux** | Windows dùng `SE Asia Standard Time`, Linux dùng chuẩn IANA `Asia/Ho_Chi_Minh`. | Code `DateHelper.cs` (Phase 2) đã có cơ chế Try-Catch quét nhiều OS. | Gọi hàm thử không ném exception. |

---
*(End of Runbook)*
