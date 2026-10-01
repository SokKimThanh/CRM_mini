# RUNBOOK NGÀY 4 — SCAFFOLD ENTITY & SEED DATA

**Sprint:** 1 · **Thời lượng dự kiến:** 120 phút

**Đường dẫn lưu trữ:** `docs/sprints/sprint_1/runbooks/day_4_scaffold_seed.md`

**Mục tiêu:**

1. Scaffold tự động 16 entity từ PostgreSQL schema Day 2 sang `Crm.Domain` (không làm đè hoặc gãy cấu hình Identity của Day 3).

2. Chuẩn hóa `AppDbContext`: Mapping 18 `DbSet`, cài đặt Snake_case naming convention và bộ chuyển đổi UTC `ValueConverter`.

3. Chạy dữ liệu mẫu (`04_demo_data.sql`): 20 khách hàng, 30 liên hệ, 20 sản phẩm, 15 cơ hội bán hàng, 45 lịch sử giai đoạn và phân quyền sales.

4. Khởi tạo 5 Enums nghiệp vụ và 2 Helpers cốt lõi.

## BẢNG ĐỐI CHIẾU DỮ LIỆU ĐÍCH

| 

| **Hạng mục** | **Số lượng kỳ vọng** | **Chi tiết** | 
| Entities trong `Crm.Domain` | 19 | 3 Identity/User (Day 3) + 16 Business entities | 
| `DbSet` trong `AppDbContext` | 18 | `UserProfiles`, `Teams` + 16 Business entities | 
| Customers | 20 | 5 New, 10 Healthy, 5 NeedAttention | 
| Contacts | 30 | Đầy đủ email, số điện thoại, cờ `is_primary` | 
| Products | 20 | Phân bổ qua 4 danh mục (id 1-4) | 
| Opportunities | 15 | Phân bổ đều qua 6 giai đoạn bán hàng | 
| Stage Histories | 45 | Lịch sử dịch chuyển pipeline | 
| Assigned Customers | 20 | 7 cho `sales1`, 6 cho `sales2`, 7 cho `manager` | 

## ĐIỀU KIỆN TIÊN QUYẾT (PRE-FLIGHT CHECK)

Thực thi kiểm tra trước khi chạy bất kỳ lệnh nào:

```
# 1. Kiểm tra kết nối DB và số lượng bảng Day 3 (Kỳ vọng: 25 bảng)
psql -h localhost -U crm_user -d crm_db -c "\dt"

# 2. Kiểm tra dữ liệu master data danh mục và giai đoạn
psql -h localhost -U crm_user -d crm_db -c "SELECT id, name FROM categories ORDER BY id;"
# Kỳ vọng: 4 dòng (id: 1, 2, 3, 4)

psql -h localhost -U crm_user -d crm_db -c "SELECT id, name, is_won, is_lost FROM opportunity_stages ORDER BY id;"
# Kỳ vọng: 6 dòng (id: 1..6)

# 3. Kiểm tra danh sách User Identity Day 3
psql -h localhost -U crm_user -d crm_db -c "SELECT \"Email\" FROM \"AspNetUsers\" ORDER BY \"Email\";"
# Kỳ vọng: Có admin@crm.local, manager@crm.local, sales1@crm.local, sales2@crm.local, accountant@crm.local

# 4. Kiểm tra build solution
dotnet build
# Kỳ vọng: Build succeeded. 0 Error(s).

```

## PHASE 0 — BACKUP & BẢO VỆ MÔI TRƯỜNG (5 phút)

Tránh ghi đè file tạm vào Git repository:

```
cd "F:\Design Web\PROJECT CRM MINI\CrmSolution"

# Thêm thư mục scaffold tạm thời vào .gitignore
Add-Content .gitignore "`nsrc/Crm.Data/ScaffoldTemp/"

# Lưu checkpoint Git Day 3 an toàn
git add .
git commit -m "chore: checkpoint backup before executing Day 4 scaffold"

```

## PHASE 1 — THIẾT LẬP CÔNG CỤ (5 phút)

Cài đặt Entity Framework Core CLI Tool và thư viện thiết kế:

```
# Cài đặt hoặc cập nhật dotnet-ef global tool
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet ef --version

# Thêm Design package vào Data layer
cd src\Crm.Data
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0
cd ../..

# Khôi phục và build thử
dotnet restore
dotnet build

```

## PHASE 2 — REVERSE ENGINEERING (SCAFFOLD 16 ENTITIES) (15 phút)

> ⚠️ **Quy tắc bất khả xâm phạm:**
>
> 1. Không dùng cờ `-f` ghi đè thẳng `AppDbContext` hiện có.
>
> 2. Đặt context tạm thời là `ScaffoldTempContext`.
>
> 3. Chỉ scaffold đúng 16 bảng nghiệp vụ, loại trừ các bảng Identity (`AspNet*`).

```
cd src\Crm.Data

dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=crm_db;Username=crm_user;Password=Crm@2026#Dev" Npgsql.EntityFrameworkCore.PostgreSQL `
  --output-dir ScaffoldTemp `
  --context-dir ScaffoldTemp `
  --context ScaffoldTempContext `
  --no-onconfiguring `
  --data-annotations `
  --table customers `
  --table contacts `
  --table customer_assignments `
  --table opportunities `
  --table opportunity_stages `
  --table stage_histories `
  --table categories `
  --table products `
  --table quotes `
  --table quote_items `
  --table sales_tasks `
  --table interactions `
  --table notifications `
  --table dashboard_snapshots `
  --table audit_logs `
  --table kiotviet_sync_logs

cd ../..

# Xác nhận kết quả sinh file
Get-ChildItem "src\Crm.Data\ScaffoldTemp" | Select-Object Name
# Kỳ vọng: 17 file (16 models + ScaffoldTempContext.cs)

```

## PHASE 3 — CHUẨN HÓA VÀ ĐƯA VÀO CRM.DOMAIN (15 phút)

### 3.1 Di chuyển file sang `Crm.Domain/Entities` và cập nhật namespace

```
# 1. Di chuyển các model, bỏ qua file Context tạm
Get-ChildItem "src\Crm.Data\ScaffoldTemp\*.cs" | Where-Object { $_.Name -ne "ScaffoldTempContext.cs" } | Copy-Item -Destination "src\Crm.Domain\Entities\" -Force

# 2. Thay đổi namespace từ ScaffoldTemp sang Crm.Domain.Entities
$preserveFiles = @("ApplicationUser.cs", "UserProfile.cs", "Team.cs")
Get-ChildItem "src\Crm.Domain\Entities\*.cs" | Where-Object { $preserveFiles -notcontains $_.Name } | ForEach-Object {
    (Get-Content $_.FullName -Raw) -replace 'namespace Crm\.Data\.ScaffoldTemp', 'namespace Crm.Domain.Entities' | Set-Content $_.FullName -Encoding utf8
}

# 3. Kiểm tra rà soát namespace sót lại
Select-String -Path "src\Crm.Domain\Entities\*.cs" -Pattern "ScaffoldTemp"
# Kỳ vọng: Không có dòng nào hiển thị

```

### 3.2 Chuẩn hóa lỗi đặt tên PascalCase `KiotvietSyncLog`

EF Core scaffold tự động bảng `kiotviet_sync_logs` thành `KiotvietSyncLog`. Ta phải đổi thành `KiotVietSyncLog` để đồng bộ code convention.

```
Rename-Item "src\Crm.Domain\Entities\KiotvietSyncLog.cs" "KiotVietSyncLog.cs"
(Get-Content "src\Crm.Domain\Entities\KiotVietSyncLog.cs" -Raw) -replace 'class KiotvietSyncLog', 'class KiotVietSyncLog' | Set-Content "src\Crm.Domain\Entities\KiotVietSyncLog.cs" -Encoding utf8

# Kiểm tra lại tên class
Select-String -Path "src\Crm.Domain\Entities\KiotVietSyncLog.cs" -Pattern "class Kiot"
# Kỳ vọng: public partial class KiotVietSyncLog

```

### 3.3 Dọn dẹp thư mục tạm và bổ sung dependencies

```
# Xóa thư mục ScaffoldTemp
Remove-Item "src\Crm.Data\ScaffoldTemp" -Recurse -Force

# Kiểm tra tổng số entity trong Domain
(Get-ChildItem "src\Crm.Domain\Entities").Count
# Kỳ vọng: 19 file

# Bổ sung EF Core package vào Domain nếu các entity dùng data annotations ([Index], [Precision])
cd src\Crm.Domain
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.0
cd ../..

```

## PHASE 4 — TÁI CẤU TRÚC APPDBCONTEXT (20 phút)

Ghi đè nội dung file `src\Crm.Data\AppDbContext.cs` với cấu hình DbSets, Snake_Case naming convention và UTC converter tự động:

```
using System.Text;
using Crm.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Crm.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Day 3: Identity & Team Models
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Team> Teams => Set<Team>();

    // Day 4: Core Business Models
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<CustomerAssignment> CustomerAssignments => Set<CustomerAssignment>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<OpportunityStage> OpportunityStages => Set<OpportunityStage>();
    public DbSet<StageHistory> StageHistories => Set<StageHistory>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();
    public DbSet<SalesTask> SalesTasks => Set<SalesTask>();
    public DbSet<Interaction> Interactions => Set<Interaction>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DashboardSnapshot> DashboardSnapshots => Set<DashboardSnapshot>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<KiotVietSyncLog> KiotVietSyncLogs => Set<KiotVietSyncLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // ⚠️ BẮT BUỘC: Giữ base configuration cho ASP.NET Identity
        base.OnModelCreating(builder);

        // 1. Cấu hình bảng UserProfile (Day 3)
        builder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.EmployeeCode).HasMaxLength(20);
            entity.Property(u => u.RoleCode).IsRequired().HasMaxLength(20);

            entity.HasOne(u => u.User)
                  .WithOne()
                  .HasForeignKey<UserProfile>(u => u.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.Team)
                  .WithMany()
                  .HasForeignKey(u => u.TeamId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 2. Cấu hình bảng Team (Day 3)
        builder.Entity<Team>(entity =>
        {
            entity.ToTable("teams");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);

            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(t => t.ManagerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 3. Tự động chuẩn hóa Snake_case và ValueConverter UTC cho Domain Entities
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableUtcConverter = new ValueConverter<DateTime?, DateTime?>(
            v => !v.HasValue ? v : (v.Value.Kind == DateTimeKind.Utc ? v : v.Value.ToUniversalTime()),
            v => !v.HasValue ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // Chỉ áp dụng với Entity do ta định nghĩa, không chạm vào Identity tables
            if (entity.ClrType.Namespace?.StartsWith("Crm.Domain.Entities") != true)
                continue;

            // Xử lý Column Names & DateTime Conversion
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));

                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(utcConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableUtcConverter);
                }
            }

            // Xử lý Keys, Foreign Keys, Indexes
            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()!));
            }

            foreach (var fk in entity.GetForeignKeys())
            {
                fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()!));
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()!));
            }
        }
    }

    /// <summary>
    /// Chuyển đổi tên thuộc tính PascalCase sang snake_case của PostgreSQL.
    /// Giữ nguyên quy tắc cho các tiền tố PK/FK ("PK_customers" -> "pk_customers").
    /// </summary>
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            char c = input[i];
            if (char.IsUpper(c) && (char.IsLower(input[i - 1]) || char.IsDigit(input[i - 1])))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(char.ToLowerInvariant(c));
            }
        }
        return sb.ToString();
    }
}

```

Kiểm tra build:

```
dotnet build
# Kỳ vọng: Build succeeded. 0 Warning(s), 0 Error(s).

```

## PHASE 4.5 — SMOKE TEST BẢO TOÀN DANH TÍNH (5 phút)

Xác minh cấu hình mới không làm vỡ hệ thống Identity và Blazor UI của Day 3:

```
cd src\Crm.Web
dotnet run

```

Thao tác kiểm tra tại trình duyệt (`https://localhost:7059`):

1. Truy cập trang chủ $\rightarrow$ Tự động chuyển hướng về `/login`.

2. Giao diện login MudBlazor hiển thị đầy đủ styling.

3. Đăng nhập với tài khoản: `admin@crm.local` / `Admin@2026`.

4. Đăng nhập thành công và chuyển vào Dashboard.

5. Kiểm tra terminal: **Không có runtime exception** hoặc lỗi missing column.

Nhấn `Ctrl + C` để dừng app trước khi seed data.

## PHASE 5 — DEMO DATA SEEDING (35 phút)

### 5.1 Tạo file kiểm tra điều kiện: `docs/db/precheck_day4.sql`

```
-- Kiểm tra sự sẵn sàng của Master Data trước khi nạp demo
SELECT 'categories_count' AS check_item, COUNT(*) AS actual, 4 AS expected FROM categories
UNION ALL
SELECT 'stages_count', COUNT(*), 6 FROM opportunity_stages
UNION ALL
SELECT 'users_count', COUNT(*), 5 FROM "AspNetUsers";

```

### 5.2 Tạo kịch bản dữ liệu hoàn chỉnh: `docs/db/04_demo_data.sql`

```
-- =====================================================================
-- CRM SEED DATA SCRIPT - DAY 4
-- Mục tiêu: Nạp dữ liệu giả lập cho quy trình bán hàng B2B cơ khí
-- Lưu ý: Chỉ thực thi trong môi trường Development.
-- =====================================================================

-- 1. Làm sạch dữ liệu các bảng nghiệp vụ theo đúng thứ tự ràng buộc
TRUNCATE TABLE 
    interactions,
    stage_histories,
    sales_tasks,
    quote_items,
    quotes,
    opportunities,
    customer_assignments,
    contacts,
    customers,
    products
RESTART IDENTITY CASCADE;

-- 2. Thêm 20 khách hàng B2B
-- Trạng thái sức khỏe (health_status): 0=New, 1=Healthy, 2=NeedAttention, 3=AtRisk, 4=Dormant, 5=Churned
INSERT INTO customers (id, code, name, industry, tax_code, phone, email, address, health_status, average_cycle_days, revenue_90d, order_count_90d) VALUES
(1, 'KH-0001', 'Công ty TNHH Cơ khí An Phát', 'Gia công cơ khí', '0101234567', '0912345678', 'contact@anphat.vn', 'KCN Quang Minh, Mê Linh, Hà Nội', 1, 30, 145000000, 4),
(2, 'KH-0002', 'Cơ khí Chính xác Việt Nhật', 'Chế tạo máy', '0102345678', '0913456789', 'info@vietnhat.vn', 'KCN Thăng Long, Đông Anh, Hà Nội', 1, 45, 280000000, 5),
(3, 'KH-0003', 'Khuôn Mẫu Tân Á', 'Khuôn mẫu', '0103456789', '0914567890', 'sales@tana.vn', 'KCN Tân Tạo, Bình Tân, TP.HCM', 2, 60, 42000000, 1),
(4, 'KH-0004', 'Bulong Ốc Vít Nam An', 'Sản xuất ốc vít', '0104567890', '0915678901', 'naman@ocvit.vn', 'KCN Biên Hòa, Đồng Nai', 0, 30, 0, 0),
(5, 'KH-0005', 'Dụng Cụ Hải Nam', 'Dụng cụ cầm tay', '0105678901', '0916789012', 'hainam@tools.vn', 'KCN Long Hậu, Long An', 0, 0, 0, 0),
(6, 'KH-0006', 'Cơ khí Thắng Lợi', 'Gia công CNC', '0106789012', '0917890123', 'info@thangloi.vn', 'KCN Sài Đồng, Long Biên, Hà Nội', 1, 30, 195000000, 3),
(7, 'KH-0007', 'Chế Tạo Máy Minh Đức', 'Chế tạo máy', '0107890123', '0918901234', 'minhduc@ctm.vn', 'KCN Từ Liêm, Hà Nội', 1, 60, 95000000, 2),
(8, 'KH-0008', 'Bơm Công Nghiệp Sài Gòn', 'Sản xuất bơm', '0108901234', '0919012345', 'saigon@pump.vn', 'KCN Tân Bình, TP.HCM', 2, 45, 68000000, 1),
(9, 'KH-0009', 'Cơ khí Hoàng Long', 'Gia công cơ khí', '0109012345', '0920123456', 'hoanglong@ck.vn', 'KCN VSIP, Bình Dương', 1, 30, 210000000, 6),
(10, 'KH-0010', 'Kỹ Thuật Tân Hưng', 'Kỹ thuật CN', '0110123456', '0921234567', 'tanhung@kt.vn', 'KCN Amata, Biên Hòa', 2, 60, 35000000, 1),
(11, 'KH-0011', 'Cơ Điện Lạnh Bình Minh', 'Cơ điện lạnh', '0111234567', '0922345678', 'binhminh@cdl.vn', 'Q. Tân Phú, TP.HCM', 0, 30, 0, 0),
(12, 'KH-0012', 'Đúc Kim Loại Phú Thịnh', 'Đúc kim loại', '0112345678', '0923456789', 'phuthinh@dk.vn', 'KCN Hiệp Phước, TP.HCM', 1, 45, 178000000, 4),
(13, 'KH-0013', 'Cơ khí Đông Á', 'Gia công', '0113456789', '0924567890', 'donga@ck.vn', 'KCN Đình Vũ, Hải Phòng', 1, 30, 240000000, 5),
(14, 'KH-0014', 'Máy Công Cụ Hà Nội', 'Máy công cụ', '0114567890', '0925678901', 'hn@machine.vn', 'Q. Hai Bà Trưng, Hà Nội', 2, 60, 52000000, 1),
(15, 'KH-0015', 'Thiết Bị Công Nghiệp Việt', 'Thiết bị CN', '0115678901', '0926789012', 'viet@tbcn.vn', 'KCN Long Bình, Đồng Nai', 0, 0, 0, 0),
(16, 'KH-0016', 'Cơ khí Đại Việt', 'Gia công CNC', '0116789012', '0927890123', 'daiviet@ck.vn', 'KCN Tân Đô, Long An', 1, 30, 128000000, 3),
(17, 'KH-0017', 'Khuôn Chính Xác Sài Gòn', 'Khuôn mẫu', '0117890123', '0928901234', 'saigon@khuon.vn', 'Q. Bình Thạnh, TP.HCM', 1, 45, 165000000, 3),
(18, 'KH-0018', 'Cơ khí Phương Đông', 'Gia công cơ khí', '0118901234', '0929012345', 'phuongdong@ck.vn', 'KCN Phú Mỹ, Bà Rịa - VT', 2, 30, 45000000, 1),
(19, 'KH-0019', 'Chế Tạo Khuôn Nam Phát', 'Khuôn mẫu', '0119012345', '0930123456', 'namphat@khuon.vn', 'KCN Tân Kim, Long An', 0, 30, 0, 0),
(20, 'KH-0020', 'Cơ khí Trường Thịnh', 'Gia công cơ khí', '0120123456', '0931234567', 'truongthinh@ck.vn', 'KCN Đồng Văn, Hà Nam', 1, 30, 220000000, 5);

SELECT setval('customers_id_seq', 20, true);

-- 3. Thêm 30 đầu mối liên hệ (Contacts)
INSERT INTO contacts (id, customer_id, name, position, phone, email, is_primary) VALUES
(1, 1, 'Trần Văn Cường', 'Trưởng phòng thu mua', '0912345001', 'cuong@anphat.vn', TRUE),
(2, 1, 'Lê Thị Mai', 'Kế toán trưởng', '0912345002', 'mai@anphat.vn', FALSE),
(3, 2, 'Nguyễn Thị Hà', 'Kế toán trưởng', '0912345003', 'ha@vietnhat.vn', TRUE),
(4, 2, 'Phạm Văn Hùng', 'Kỹ thuật', '0912345004', 'hung@vietnhat.vn', FALSE),
(5, 3, 'Lê Văn Tuấn', 'Giám đốc kỹ thuật', '0912345005', 'tuan@tana.vn', TRUE),
(6, 4, 'Phạm Minh Đức', 'Nhân viên thu mua', '0912345006', 'duc@ocvit.vn', TRUE),
(7, 5, 'Hoàng Văn Nam', 'Chủ doanh nghiệp', '0912345007', 'nam@tools.vn', TRUE),
(8, 6, 'Đỗ Văn Khoa', 'Trưởng phòng KT', '0912345008', 'khoa@thangloi.vn', TRUE),
(9, 6, 'Nguyễn Văn Long', 'Thủ kho', '0912345009', 'long@thangloi.vn', FALSE),
(10, 7, 'Trịnh Văn Bình', 'Giám đốc', '0912345010', 'binh@ctm.vn', TRUE),
(11, 8, 'Vũ Thị Hương', 'Trưởng phòng mua', '0912345011', 'huong@pump.vn', TRUE),
(12, 9, 'Lý Văn Hải', 'Trưởng phòng KT', '0912345012', 'hai@hoanglong.vn', TRUE),
(13, 9, 'Ngô Thị Lan', 'Kế toán', '0912345013', 'lan@hoanglong.vn', FALSE),
(14, 10, 'Dương Văn Tùng', 'Giám đốc', '0912345014', 'tung@tanhung.vn', TRUE),
(15, 11, 'Trần Đình Quang', 'Trưởng phòng mua', '0912345015', 'quang@binhminh.vn', TRUE),
(16, 12, 'Hoàng Văn Sơn', 'Trưởng phòng KT', '0912345016', 'son@phuthinh.vn', TRUE),
(17, 13, 'Lê Đình Nam', 'Giám đốc kỹ thuật', '0912345017', 'nam@donga.vn', TRUE),
(18, 13, 'Nguyễn Thị Thu', 'Kế toán', '0912345018', 'thu@donga.vn', FALSE),
(19, 14, 'Bùi Văn Tài', 'Trưởng phòng mua', '0912345019', 'tai@machine.vn', TRUE),
(20, 15, 'Nguyễn Văn Bình', 'Chủ tịch', '0912345020', 'binh@tbcn.vn', TRUE),
(21, 16, 'Trần Quốc Bảo', 'TP Kinh doanh', '0912345021', 'bao@daiviet.vn', TRUE),
(22, 17, 'Phan Văn Hòa', 'Giám đốc', '0912345022', 'hoa@khuon.vn', TRUE),
(23, 18, 'Đặng Văn Lâm', 'Trưởng phòng KT', '0912345023', 'lam@phuongdong.vn', TRUE),
(24, 19, 'Võ Văn Đông', 'Chủ doanh nghiệp', '0912345024', 'dong@namphat.vn', TRUE),
(25, 20, 'Nguyễn Văn Hóa', 'Giám đốc', '0912345025', 'hoa@truongthinh.vn', TRUE),
(26, 20, 'Trần Thị Hồng', 'Kế toán', '0912345026', 'hong@truongthinh.vn', FALSE),
(27, 2, 'Lê Văn Đức', 'Thủ kho', '0912345027', 'duc@vietnhat.vn', FALSE),
(28, 7, 'Phạm Thị Xuân', 'Kế toán', '0912345028', 'xuan@ctm.vn', FALSE),
(29, 12, 'Bùi Văn Hậu', 'Kỹ thuật', '0912345029', 'hau@phuthinh.vn', FALSE),
(30, 16, 'Nguyễn Văn Quý', 'Thủ kho', '0912345030', 'quy@daiviet.vn', FALSE);

SELECT setval('contacts_id_seq', 30, true);

-- 4. Thêm 20 sản phẩm theo 4 nhóm Category
INSERT INTO products (id, code, name, category_id, unit, base_price, cost_price) VALUES
(1, 'DP-001', 'Dao phay Ø10 HSS', 1, 'Cái', 450000, 320000),
(2, 'DP-002', 'Mũi khoan mạ Titan Ø5', 1, 'Cây', 85000, 60000),
(3, 'DP-003', 'Mảnh tiện CNC Kyocera CNMG', 1, 'Hộp', 1200000, 850000),
(4, 'DP-004', 'Dao tiện ngoài 16x16mm', 1, 'Cây', 580000, 420000),
(5, 'DP-005', 'Mũi phay ngón Ø12', 1, 'Cây', 380000, 270000),
(6, 'DM-001', 'Đá mài 100x6x16', 2, 'Viên', 45000, 30000),
(7, 'DM-002', 'Đĩa cắt sắt 105x1.2', 2, 'Cái', 15000, 10000),
(8, 'DM-003', 'Bánh mài nỉ 150mm', 2, 'Cái', 125000, 85000),
(9, 'DM-004', 'Đá mài bàn 150x20x32', 2, 'Viên', 95000, 68000),
(10, 'DM-005', 'Đĩa cắt inox 125x1.0', 2, 'Cái', 18000, 12000),
(11, 'OV-001', 'Bulong M8x30 inox', 3, 'Con', 5000, 3200),
(12, 'OV-002', 'Đai ốc M10 thép', 3, 'Con', 3500, 2200),
(13, 'OV-003', 'Vít đầu dù M6x20', 3, 'Con', 2800, 1800),
(14, 'OV-004', 'Long đền phẳng M8', 3, 'Cái', 800, 500),
(15, 'OV-005', 'Bulong M12x50 inox 304', 3, 'Con', 12000, 8500),
(16, 'DB-001', 'Dầu cắt gọt KoolKut 20L', 4, 'Can', 650000, 480000),
(17, 'DB-002', 'Mỡ bôi trơn công nghiệp 1kg', 4, 'Hộp', 180000, 120000),
(18, 'DB-003', 'Dầu thủy lực AW68 200L', 4, 'Phuy', 8500000, 6800000),
(19, 'DB-004', 'Dầu làm mát CNC 20L', 4, 'Can', 720000, 550000),
(20, 'DB-005', 'Mỡ chịu nhiệt cao 500g', 4, 'Tuýp', 95000, 65000);

SELECT setval('products_id_seq', 20, true);

-- 5. Thêm 15 cơ hội bán hàng (Opportunities)
INSERT INTO opportunities (id, code, customer_id, contact_id, stage_id, title, estimated_value, probability, expected_close_date, source) VALUES
(1, 'OPP-0001', 1, 1, 1, 'Lô dao phay tháng 10', 35000000, 10, NOW() + INTERVAL '30 days', 'Triển lãm'),
(2, 'OPP-0002', 2, 3, 1, 'Cung cấp mũi khoan quý 4', 45000000, 10, NOW() + INTERVAL '25 days', 'Gọi điện'),
(3, 'OPP-0003', 3, 5, 2, 'Tư vấn đá mài cho xưởng khuôn', 28000000, 30, NOW() + INTERVAL '40 days', 'Giới thiệu'),
(4, 'OPP-0004', 6, 8, 2, 'Bulong inox cho dây chuyền', 65000000, 30, NOW() + INTERVAL '35 days', 'Web'),
(5, 'OPP-0005', 9, 12, 3, 'Báo giá dầu cắt gọt KoolKut', 35000000, 50, NOW() + INTERVAL '20 days', 'Gọi điện'),
(6, 'OPP-0006', 13, 17, 3, 'Đá mài bàn cho xưởng mới', 42000000, 50, NOW() + INTERVAL '22 days', 'Giới thiệu'),
(7, 'OPP-0007', 4, 6, 4, 'Đàm phán hợp đồng năm 2027', 180000000, 70, NOW() + INTERVAL '60 days', 'Web'),
(8, 'OPP-0008', 7, 10, 4, 'Dầu thủy lực AW68 số lượng lớn', 170000000, 70, NOW() + INTERVAL '55 days', 'Triển lãm'),
(9, 'OPP-0009', 5, 7, 5, 'Đơn đầu tiên dụng cụ cầm tay', 25000000, 100, NOW() - INTERVAL '5 days', 'Walk-in'),
(10, 'OPP-0010', 16, 21, 5, 'Mũi phay ngón Ø12 - 100 cây', 38000000, 100, NOW() - INTERVAL '3 days', 'Gọi điện'),
(11, 'OPP-0011', 8, 11, 6, 'Bơm công nghiệp - thua đối thủ', 95000000, 0, NOW() - INTERVAL '10 days', 'Web'),
(12, 'OPP-0012', 10, 14, 6, 'Kỹ thuật Tân Hưng - giá cao', 42000000, 0, NOW() - INTERVAL '8 days', 'Giới thiệu'),
(13, 'OPP-0013', 12, 16, 4, 'Đúc Phú Thịnh - đang đàm phán', 145000000, 70, NOW() + INTERVAL '45 days', 'Triển lãm'),
(14, 'OPP-0014', 17, 22, 3, 'Khuôn Sài Gòn - báo giá mảnh tiện', 52000000, 50, NOW() + INTERVAL '18 days', 'Web'),
(15, 'OPP-0015', 20, 25, 2, 'Trường Thịnh - cần dầu làm mát', 68000000, 30, NOW() + INTERVAL '38 days', 'Gọi điện');

SELECT setval('opportunities_id_seq', 15, true);

-- 6. Thêm 45 lịch sử chuyển giai đoạn (Stage Histories)
INSERT INTO stage_histories (opportunity_id, from_stage_id, to_stage_id, note) VALUES
(1, NULL, 1, 'Tạo cơ hội mới'),
(2, NULL, 1, 'Tạo cơ hội mới'),
(3, NULL, 1, 'Tạo cơ hội mới'),
(3, 1, 2, 'Gặp trực tiếp khách hàng'),
(4, NULL, 1, 'Tạo cơ hội mới'),
(4, 1, 2, 'Đã kết nối qua điện thoại'),
(5, NULL, 1, 'Tạo cơ hội mới'),
(5, 1, 2, 'Gọi điện tư vấn thông số dầu cắt'),
(5, 2, 3, 'Đã gửi báo giá chi tiết qua email'),
(6, NULL, 1, 'Tạo cơ hội mới'),
(6, 1, 2, 'Khảo sát nhu cầu thực tế xưởng'),
(6, 2, 3, 'Đã chốt danh sách sản phẩm và gửi báo giá'),
(7, NULL, 1, 'Tạo cơ hội mới'),
(7, 1, 2, 'Đàm phán sơ bộ về chính sách chiết khấu'),
(7, 2, 3, 'Báo giá chính thức theo khối lượng năm'),
(7, 3, 4, 'Bắt đầu vòng đàm phán hợp đồng khung'),
(8, NULL, 1, 'Tạo cơ hội mới'),
(8, 1, 2, 'Khảo sát hệ thống thủy lực nhà máy'),
(8, 2, 3, 'Báo giá gói 20 phuy dầu AW68'),
(8, 3, 4, 'Đàm phán điều khoản thanh toán 60 ngày'),
(9, NULL, 1, 'Tạo cơ hội mới'),
(9, 1, 2, 'Tiếp khách tại showroom'),
(9, 2, 3, 'Gửi báo giá dụng cụ cầm tay'),
(9, 3, 4, 'Thống nhất bảng giá ưu đãi mở điểm bán'),
(9, 4, 5, 'Ký hợp đồng và nhận tiền đặt cọc'),
(10, NULL, 1, 'Tạo cơ hội mới'),
(10, 1, 2, 'Khách hàng gọi đặt bổ sung vật tư tiêu hao'),
(10, 2, 3, 'Gửi báo giá giao ngay trong ngày'),
(10, 3, 4, 'Khách duyệt báo giá qua Zalo'),
(10, 4, 5, 'Xuất kho và hoàn tất thanh toán tiền mặt'),
(11, NULL, 1, 'Tạo cơ hội mới'),
(11, 1, 2, 'Liên hệ phòng kỹ thuật'),
(11, 2, 6, 'Thua đối thủ địa phương về giá và chi phí vận chuyển'),
(12, NULL, 1, 'Tạo cơ hội mới'),
(12, 1, 2, 'Gặp trực tiếp quản đốc'),
(12, 2, 6, 'Ngân sách công ty khách bị cắt giảm'),
(13, NULL, 1, 'Tạo cơ hội mới'),
(13, 1, 2, 'Khảo sát nhà máy đúc Phú Thịnh'),
(13, 2, 3, 'Gửi bảng giá đại lý cấp 1'),
(13, 3, 4, 'Đang làm rõ điều khoản công nợ gối đầu'),
(14, NULL, 1, 'Tạo cơ hội mới'),
(14, 1, 2, 'Liên hệ qua thông tin để lại trên website'),
(14, 2, 3, 'Gửi báo giá mảnh tiện Kyocera'),
(15, NULL, 1, 'Tạo cơ hội mới'),
(15, 1, 2, 'Gọi điện trao đổi quy cách dầu làm mát');

-- 7. Phân bổ khách hàng cho Sales Reps phục vụ kiểm thử RBAC
UPDATE customers 
SET assigned_to_user_id = (SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'sales1@crm.local')
WHERE code IN ('KH-0001','KH-0002','KH-0006','KH-0009','KH-0013','KH-0016','KH-0020');

UPDATE customers 
SET assigned_to_user_id = (SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'sales2@crm.local')
WHERE code IN ('KH-0003','KH-0004','KH-0007','KH-0010','KH-0012','KH-0017');

UPDATE customers 
SET assigned_to_user_id = (SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'manager@crm.local')
WHERE assigned_to_user_id IS NULL;

```

### 5.3 Chạy nạp dữ liệu qua PowerShell

```
$env:PGCLIENTENCODING = "UTF8"
$env:PGPASSWORD = "Crm@2026#Dev"

# Chạy Precheck
psql -h localhost -U crm_user -d crm_db -f docs/db/precheck_day4.sql

# Chạy Seed Data chính thức
psql -h localhost -U crm_user -d crm_db -f docs/db/04_demo_data.sql

```

### 5.4 Tạo file và chạy xác thực: `docs/db/verify_day4.sql`

```
-- 1. Kiểm tra tổng số lượng bản ghi các bảng chính
SELECT 'Customers' AS table_name, COUNT(*) AS count_val, 20 AS target_val FROM customers
UNION ALL SELECT 'Contacts', COUNT(*), 30 FROM contacts
UNION ALL SELECT 'Products', COUNT(*), 20 FROM products
UNION ALL SELECT 'Opportunities', COUNT(*), 15 FROM opportunities
UNION ALL SELECT 'Stage Histories', COUNT(*), 45 FROM stage_histories
UNION ALL SELECT 'Assigned Customers', COUNT(*), 20 FROM customers WHERE assigned_to_user_id IS NOT NULL;

-- 2. Kiểm tra phân bổ Opportunities theo Pipeline Stage
SELECT 
    s.id AS stage_id,
    s.name AS stage_name,
    COUNT(o.id) AS opp_count,
    COALESCE(SUM(o.estimated_value), 0) AS total_value
FROM opportunity_stages s
LEFT JOIN opportunities o ON o.stage_id = s.id
GROUP BY s.id, s.name
ORDER BY s.id;

-- 3. Kiểm tra phân bổ Customer Health
SELECT health_status, COUNT(*) AS count_val 
FROM customers 
GROUP BY health_status 
ORDER BY health_status;

-- 4. Kiểm tra phân bổ nhân sự phụ trách
SELECT u."Email", COUNT(c.id) AS assigned_customer_count
FROM "AspNetUsers" u
JOIN customers c ON c.assigned_to_user_id = u."Id"
GROUP BY u."Email"
ORDER BY assigned_customer_count DESC;

```

Thực thi kiểm tra:

```
psql -h localhost -U crm_user -d crm_db -f docs/db/verify_day4.sql

```

**Bảng chuẩn đối chiếu kết quả Verify:**

1. **Tổng lượng bản ghi:** Customers = 20, Contacts = 30, Products = 20, Opportunities = 15, Stage Histories = 45, Assigned = 20.

2. **Giá trị cơ hội bán hàng:**

   * Stage 1 (Mới tiếp cận): 2 cơ hội | 80,000,000 đ

   * Stage 2 (Đang liên hệ): 3 cơ hội | 161,000,000 đ

   * Stage 3 (Đã gửi báo giá): 3 cơ hội | 129,000,000 đ

   * Stage 4 (Đang đàm phán): 3 cơ hội | 495,000,000 đ

   * Stage 5 (Chốt thành công): 2 cơ hội | 63,000,000 đ

   * Stage 6 (Thất bại): 2 cơ hội | 137,000,000 đ

   * **Tổng cộng: 15 cơ hội | 1,065,000,000 đ**

3. **Phân bổ nhân sự:**

   * `sales1@crm.local`: 7 khách hàng

   * `manager@crm.local`: 7 khách hàng

   * `sales2@crm.local`: 6 khách hàng

## PHASE 6 — ENUMS VÀ BUSINESS HELPERS (15 phút)

Tạo cấu trúc thư mục nếu chưa tồn tại:

```
New-Item -ItemType Directory -Force -Path "src\Crm.Domain\Enums"
New-Item -ItemType Directory -Force -Path "src\Crm.Business\Helpers"

```

### 6.1 Tạo các Enums nghiệp vụ (`src/Crm.Domain/Enums/`)

**1. `src/Crm.Domain/Enums/CustomerHealth.cs`:**

```
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

```

**2. `src/Crm.Domain/Enums/SalesTaskStatus.cs`:**

```
namespace Crm.Domain.Enums;

/// <summary>
/// Trạng thái công việc của Sales (Đặt tên SalesTaskStatus để tránh xung đột với System.Threading.Tasks.TaskStatus).
/// </summary>
public enum SalesTaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Skipped = 3,
    Cancelled = 4
}

```

**3. `src/Crm.Domain/Enums/TaskOutcome.cs`:**

```
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

```

**4. `src/Crm.Domain/Enums/QuoteStatus.cs`:**

```
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

```

**5. `src/Crm.Domain/Enums/InteractionType.cs`:**

```
namespace Crm.Domain.Enums;

public enum InteractionType
{
    Call = 1,
    Meeting = 2,
    Email = 3,
    Zalo = 4,
    Visit = 5
}

```

### 6.2 Tạo các Business Helpers (`src/Crm.Business/Helpers/`)

**1. `src/Crm.Business/Helpers/CurrencyHelper.cs`:**

```
using System.Globalization;

namespace Crm.Business.Helpers;

public static class CurrencyHelper
{
    private static readonly CultureInfo VietnamCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string Format(decimal amount)
        => amount.ToString("N0", VietnamCulture) + " đ";

    public static string FormatShort(decimal amount)
    {
        if (amount >= 1_000_000_000) return $"{amount / 1_000_000_000:0.##} tỷ";
        if (amount >= 1_000_000) return $"{amount / 1_000_000:0.##} tr";
        if (amount >= 1_000) return $"{amount / 1_000:0.##}K";
        return amount.ToString("N0", VietnamCulture) + " đ";
    }
}

```

**2. `src/Crm.Business/Helpers/DateHelper.cs`:**

```
namespace Crm.Business.Helpers;

public static class DateHelper
{
    // Đảm bảo hoạt động trên cả Windows ("SE Asia Standard Time") và Linux/Docker ("Asia/Ho_Chi_Minh")
    private static readonly TimeZoneInfo VietnamTz = GetVietnamTimeZone();

    private static TimeZoneInfo GetVietnamTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
        }
    }

    public static DateTime ToVietnamTime(DateTime utc)
        => TimeZoneInfo.ConvertTimeFromUtc(utc, VietnamTz);

    public static DateTime ToUtc(DateTime vietnam)
        => TimeZoneInfo.ConvertTimeToUtc(vietnam, VietnamTz);

    public static int DaysBetween(DateTime from, DateTime to)
        => (to.Date - from.Date).Days;
}

```

Kiểm tra build toàn bộ Solution:

```
dotnet build
# Kỳ vọng: 0 Warning, 0 Error

```

## PHASE 7 — SMOKE TEST TOÀN DIỆN & COMMIT (15 phút)

### 7.1 Kiểm tra đăng nhập đa vai trò

Chạy web app:

```
cd src\Crm.Web
dotnet run

```

* \[ \] Đăng nhập `admin@crm.local` / `Admin@2026`: Thành công.

* \[ \] Đăng xuất và đăng nhập `sales1@crm.local` / `Sales@2026`: Thành công.

* \[ \] Không có exception liên quan đến database context trên Terminal.

### 7.2 Commit Git

```
cd "F:\Design Web\PROJECT CRM MINI\CrmSolution"

git add .
git commit -m "feat(data): Day 4 - Scaffold 16 entities + seed demo data

- Scaffolded 16 business entities without overwriting AppDbContext
- Renamed KiotvietSyncLog to KiotVietSyncLog
- Configured 18 DbSets with Snake_case convention & UTC ValueConverter
- Seeded: 20 customers, 30 contacts, 20 products, 15 opps, 45 stage histories
- Assigned 20 customers across sales1, sales2, and manager
- Added 5 domain enums and 2 business helpers (cross-platform timezone ready)
- Verified login and database consistency"

```

### 7.3 Cập nhật Daily Log

Cập nhật file `notes/daily.md`:

```
# Daily Log — Day 4 / Sprint 1

## Đã hoàn thành (Done)
- [x] Tạo checkpoint backup Git trước khi scaffold.
- [x] Scaffold thành công 16 business entities sang thư mục tạm và chuyển sang Crm.Domain.
- [x] Khắc phục tên class KiotvietSyncLog -> KiotVietSyncLog.
- [x] Thiết lập AppDbContext: 18 DbSets, Snake_Case naming convention, ValueConverter UTC.
- [x] Nạp seed data mẫu thành công: 20 khách hàng, 30 liên hệ, 20 sản phẩm, 15 cơ hội, 45 lịch sử pipeline.
- [x] Phân bổ khách hàng cho 3 tài khoản phục vụ kiểm thử RBAC.
- [x] Tạo 5 Domain Enums và 2 Business Helpers (hỗ trợ chuyển đổi timezone Windows/Linux).
- [x] Smoke test đăng nhập vai trò Admin và Sales1 thành công.

## Chỉ số kiểm tra (Metrics)
- Entities: 19 file trong Crm.Domain/Entities
- DbSets: 18 bảng trong AppDbContext
- Demo data: 20 Customers / 30 Contacts / 20 Products / 15 Opps / 45 Stage Histories
- Tổng giá trị pipeline: 1,065,000,000 VND

## Kế hoạch Ngày 5 (Next Steps)
- Xây dựng Dashboard tổng quan sử dụng MudBlazor và ApexCharts.
- Tích hợp OAuth 2.0 và cơ chế đồng bộ khách hàng/đơn hàng từ KiotViet API.

```

Đẩy code lên remote:

```
git add notes/daily.md
git commit -m "docs: complete Day 4 sprint documentation"
git push origin develop

```

## BẢNG XỬ LÝ LỖI PHỔ BIẾN (TROUBLESHOOTING)

| **Triệu chứng** | **Nguyên nhân cốt lõi** | **Cách khắc phục triệt để** | 
| `dotnet ef: command not found` | Biến môi trường PATH chưa được cập nhật sau khi cài tool | Khởi động lại PowerShell hoặc terminal VS Code | 
| `Cannot find type [Index] or [Precision]` (CS0246) | Project `Crm.Domain` thiếu thư viện EF Core chính | Chạy `cd src\Crm.Domain; dotnet add package Microsoft.EntityFrameworkCore --version 10.0.0` | 
| `Type or namespace KiotVietSyncLog could not be found` | File sinh ra tự động có tên `KiotvietSyncLog` (chữ 'v' thường) | Đổi tên file và tên class thành `KiotVietSyncLog` (theo Phase 3.2) | 
| Lỗi font chữ tiếng Việt khi chạy `psql -f` | Session PowerShell dùng mã hóa Windows-1258 hoặc ASCII | Thiết lập `$env:PGCLIENTENCODING = "UTF8"` trước khi chạy `psql` | 
| Bị mất quyền hoặc lỗi bảng `AspNet*` | `OnModelCreating` vô tình ghi đè cấu hình Identity | Đảm bảo dòng `base.OnModelCreating(builder);` nằm ở đầu hàm `OnModelCreating` | 
| Xung đột tên `TaskStatus` | Trùng với `System.Threading.Tasks.TaskStatus` của .NET | Luôn sử dụng enum `SalesTaskStatus` cho nghiệp vụ CRM | 
| Lỗi múi giờ trên Docker/Linux | Chuỗi `"SE Asia Standard Time"` chỉ hợp lệ trên Windows | Dùng hàm `GetVietnamTimeZone()` trong `DateHelper.cs` để tự fallback về `"Asia/Ho_Chi_Minh"` | 

## BẢNG KIỂM TRA HOÀN TẤT (FINAL CHECKLIST)

* \[ \] Checkpoint Git trước khi làm việc đã commit

* \[ \] Đường dẫn `.gitignore` có `src/Crm.Data/ScaffoldTemp/`

* \[ \] Đủ 19 class entity trong `src/Crm.Domain/Entities/`

* \[ \] Không còn file nào chứa namespace `ScaffoldTemp`

* \[ \] Class `KiotVietSyncLog` viết hoa đúng chữ `V`

* \[ \] 18 `DbSet` trong `AppDbContext`

* \[ \] Snake_case converter và UTC DateTime converter đã tích hợp

* \[ \] Smoke test đăng nhập Phase 4.5 PASS (không exception)

* \[ \] Chạy `precheck_day4.sql` đạt kết quả

* \[ \] Nạp thành công `04_demo_data.sql`: 20 / 30 / 20 / 15 / 45

* \[ \] Phân bổ khách hàng: `sales1` (7), `sales2` (6), `manager` (7)

* \[ \] Đủ 5 Enums trong `Crm.Domain/Enums`

* \[ \] Đủ 2 Helpers trong `Crm.Business/Helpers`

* \[ \] Compile Solution không Warning, không Error

* \[ \] Đẩy nhánh lên Git remote: `git push origin develop`