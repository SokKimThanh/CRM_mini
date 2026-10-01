# RUNBOOK NGÀY 4 — SCAFFOLD ENTITY, SEED DEMO DATA & KHẮC PHỤC MÔ HÌNH THỰC TẾ

**Sprint:** 1 · **Thời lượng thực tế:** \~120 phút

**Đường dẫn lưu trữ:** `docs/sprints/sprint_1/runbooks/day_4_scaffold_seed.md`

## MỤC TIÊU PHIÊN LÀM VIỆC (OBJECTIVES)

1. **Scaffold & Chuẩn hóa Entity:** Reverse-engineering 16 bảng nghiệp vụ từ PostgreSQL `crm_db` sang `Crm.Domain.Entities`, bảo toàn 3 thực thể Identity/User của Day 3 và xử lý chuẩn đặt tên PascalCase cho `KiotVietSyncLog`.

2. **Nạp & Kiểm chứng Demo Data:** Nạp kịch bản dữ liệu hoàn chỉnh (`04_demo_data.sql`) và chạy kiểm thử đối chiếu (`verify_day4.sql`) đảm bảo 100% chỉ số khớp chuẩn.

3. **Domain Enums & Helpers:** Thiết lập 5 bộ Enum nghiệp vụ cốt lõi và 2 Helper xử lý tiền tệ VND và múi giờ Việt Nam đa nền tảng.

4. **Khắc phục lỗi mô hình thực tế:** Chẩn đoán và giải quyết triệt để cảnh báo EF Core Validation `The foreign key property 'UserProfile.TeamId1' was created in shadow state`.

5. **Smoke Test đa vai trò:** Xác thực luồng đăng nhập thành công cho cả `Admin` và `Sales Rep`, đảm bảo console khởi động sạch không còn Warning.

## BẢNG ĐỐI CHIẾU DỮ LIỆU ĐÍCH (VERIFICATION TARGETS)

| **Hạng mục** | **Chỉ số kỳ vọng** | **Chi tiết đối chiếu thực tế** | 
| **Entities trong `Crm.Domain`** | 19 | 3 Identity (`ApplicationUser`, `UserProfile`, `Team`) + 16 Business entities | 
| **Customers** | 20 | 5 New (0), 10 Healthy (1), 5 NeedAttention (2) | 
| **Contacts** | 30 | Đầy đủ email, số điện thoại, cờ `is_primary` | 
| **Products** | 20 | Phân bổ qua 4 danh mục (id 1–4) | 
| **Opportunities** | 15 | Phân bổ qua 6 giai đoạn, tổng giá trị: **1.065.000.000 đ** | 
| **Stage Histories** | 45 | Lịch sử chuyển dịch đường ống bán hàng | 
| **Phân bổ nhân sự** | 20 | 7 cho `manager@crm.local`, 7 cho `sales1@crm.local`, 6 cho `sales2@crm.local` | 

## PHASE 0 — BACKUP & CẤU HÌNH MÔI TRƯỜNG DÀI HẠN

### 0.1 Checkpoint Git an toàn

```
cd "F:\Design Web\PROJECT CRM MINI\CrmSolution"
Add-Content .gitignore "`nsrc/Crm.Data/ScaffoldTemp/"
git add .
git commit -m "chore: checkpoint backup before executing Day 4 scaffold"

```

### 0.2 Cài đặt biến môi trường vĩnh viễn trên Windows (Khắc phục lỗi gõ lại lệnh)

Để tránh việc phiên làm việc mới bị mất mật khẩu hoặc lỗi font chữ khi gọi `psql`, thiết lập biến môi trường ở cấp độ User một lần duy nhất:

```
[System.Environment]::SetEnvironmentVariable('PGCLIENTENCODING', 'UTF8', 'User')
[System.Environment]::SetEnvironmentVariable('PGPASSWORD', 'sa', 'User')

```

*(Nếu làm việc trong phiên hiện tại chưa mở lại cửa sổ mới, gán trực tiếp: `$env:PGCLIENTENCODING = "UTF8"`; `$env:PGPASSWORD = "sa"`)*

## PHASE 1 — THIẾT LẬP CÔNG CỤ & REVERSE ENGINEERING

### 1.1 Kiểm tra công cụ EF CLI

```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet ef --version

```

### 1.2 Scaffold 16 bảng nghiệp vụ vào thư mục tạm `ScaffoldTemp`

> ⚠️ **Quy tắc:** Tuyệt đối không dùng cờ `-f` ghi đè trực tiếp `AppDbContext` hiện có.

```
cd "F:\Design Web\PROJECT CRM MINI\CrmSolution\src\Crm.Data"

dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=crm_db;Username=crm_user;Password=sa" Npgsql.EntityFrameworkCore.PostgreSQL `
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

```

## PHASE 2 — DI CHUYỂN & CHUẨN HÓA MODEL VÀO CRM.DOMAIN

### 2.1 Di chuyển file và đồng bộ namespace

```
# 1. Di chuyển các thực thể sang Domain, bỏ qua file Context tạm
Get-ChildItem "src\Crm.Data\ScaffoldTemp\*.cs" | Where-Object { $_.Name -ne "ScaffoldTempContext.cs" } | Copy-Item -Destination "src\Crm.Domain\Entities\" -Force

# 2. Đồng bộ namespace thành Crm.Domain.Entities
$preserveFiles = @("ApplicationUser.cs", "UserProfile.cs", "Team.cs")
Get-ChildItem "src\Crm.Domain\Entities\*.cs" | Where-Object { $preserveFiles -notcontains $_.Name } | ForEach-Object {
    (Get-Content $_.FullName -Raw) -replace 'namespace Crm\.Data\.ScaffoldTemp', 'namespace Crm.Domain.Entities' | Set-Content $_.FullName -Encoding utf8
}

```

### 2.2 Khắc phục chuẩn đặt tên `KiotVietSyncLog`

EF Core tự động đặt tên bảng `kiotviet_sync_logs` thành `KiotvietSyncLog`. Đổi lại chữ **V** hoa:

```
Rename-Item "src\Crm.Domain\Entities\KiotvietSyncLog.cs" "KiotVietSyncLog.cs"
(Get-Content "src\Crm.Domain\Entities\KiotVietSyncLog.cs" -Raw) -replace 'class KiotvietSyncLog', 'class KiotVietSyncLog' | Set-Content "src\Crm.Domain\Entities\KiotVietSyncLog.cs" -Encoding utf8

```

### 2.3 Dọn dẹp thư mục tạm

```
Remove-Item "src\Crm.Data\ScaffoldTemp" -Recurse -Force

```

## PHASE 3 — NẠP DỮ LIỆU DEMO (SEED DATA)

### 3.1 Pre-check điều kiện tiên quyết (`docs/db/precheck_day4.sql`)

```
SELECT 'categories_count' AS check_item, COUNT(*) AS actual, 4 AS expected FROM categories
UNION ALL
SELECT 'stages_count', COUNT(*), 6 FROM opportunity_stages
UNION ALL
SELECT 'users_count', COUNT(*), 5 FROM "AspNetUsers";

```

### 3.2 Thực thi nạp dữ liệu mẫu

Nạp file `docs/db/04_demo_data.sql` với 20 khách hàng, 30 liên hệ, 20 sản phẩm, 15 cơ hội bán hàng, 45 lịch sử chuyển giai đoạn và phân bổ nhân sự phụ trách:

```
psql -h localhost -U crm_user -d crm_db -f docs/db/precheck_day4.sql
psql -h localhost -U crm_user -d crm_db -f docs/db/04_demo_data.sql

```

## PHASE 4 — KIỂM CHỨNG DỮ LIỆU & BÀI HỌC KINH NGHIỆM PSQL

### 4.1 Tạo file kiểm chứng (`docs/db/verify_day4.sql`)

```
-- 1. Tổng số bản ghi chính
SELECT 'Customers' AS table_name, COUNT(*) AS count_val, 20 AS target_val FROM customers
UNION ALL SELECT 'Contacts', COUNT(*), 30 FROM contacts
UNION ALL SELECT 'Products', COUNT(*), 20 FROM products
UNION ALL SELECT 'Opportunities', COUNT(*), 15 FROM opportunities
UNION ALL SELECT 'Stage Histories', COUNT(*), 45 FROM stage_histories
UNION ALL SELECT 'Assigned Customers', COUNT(*), 20 FROM customers WHERE assigned_to_user_id IS NOT NULL;

-- 2. Phân bổ theo giai đoạn Pipeline
SELECT 
    s.id AS stage_id,
    s.name AS stage_name,
    COUNT(o.id) AS opp_count,
    COALESCE(SUM(o.estimated_value), 0) AS total_value
FROM opportunity_stages s
LEFT JOIN opportunities o ON o.stage_id = s.id
GROUP BY s.id, s.name
ORDER BY s.id;

-- 3. Phân bổ sức khỏe khách hàng
SELECT health_status, COUNT(*) AS count_val 
FROM customers 
GROUP BY health_status 
ORDER BY health_status;

-- 4. Phân bổ khách hàng cho Sales Reps
SELECT u."Email", COUNT(c.id) AS assigned_customer_count
FROM "AspNetUsers" u
JOIN customers c ON c.assigned_to_user_id = u."Id"
GROUP BY u."Email"
ORDER BY assigned_customer_count DESC;

```

### 4.2 Bài học thực tế khi chạy lệnh Verify trên PowerShell

Khi thực thi `psql`, thường gặp 2 vấn đề hiển thị:

1. **Bị dừng ở `-- More --`:** Mặc định `psql` dùng bộ ngắt trang (pager). Khắc phục bằng cách truyền cờ `-P pager=off`.

2. **Lỗi font chữ tiếng Việt:** Console Windows mặc định dùng CodePage 437/1258. Khắc phục bằng lệnh `chcp 65001` và set `OutputEncoding`.

**Lệnh chạy chuẩn xác không ngắt trang và hiển thị tiếng Việt sắc nét:**

```
chcp 65001
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:PGCLIENTENCODING = "UTF8"

psql -h localhost -U crm_user -d crm_db -P pager=off -f docs/db/verify_day4.sql

```

**Kết quả thực tế đã nghiệm thu thành công:**

```
     table_name     | count_val | target_val 
--------------------+-----------+------------
 Customers          |        20 |         20
 Contacts           |        30 |         30
 Products           |        20 |         20
 Opportunities      |        15 |         15
 Stage Histories    |        45 |         45
 Assigned Customers |        20 |         20
(6 rows)

 stage_id |   stage_name    | opp_count | total_value  
----------+-----------------+-----------+--------------
        1 | Mới tiếp cận    |         2 |  80000000.00
        2 | Đang liên hệ    |         3 | 161000000.00
        3 | Đã gửi báo giá  |         3 | 129000000.00
        4 | Đang đàm phán   |         3 | 495000000.00
        5 | Chốt thành công |         2 |  63000000.00
        6 | Thất bại        |         2 | 137000000.00
(6 rows)

 health_status | count_val 
---------------+-----------
             0 |         5
             1 |        10
             2 |         5
(3 rows)

       Email       | assigned_customer_count 
-------------------+-------------------------
 manager@crm.local |                       7
 sales1@crm.local  |                       7
 sales2@crm.local  |                       6
(3 rows)

```

## PHASE 5 — ENUMS VÀ BUSINESS HELPERS

### 5.1 Khởi tạo thư mục

```
New-Item -ItemType Directory -Force -Path "src\Crm.Domain\Enums"
New-Item -ItemType Directory -Force -Path "src\Crm.Business\Helpers"

```

### 5.2 Khởi tạo 5 Enums nghiệp vụ (`src/Crm.Domain/Enums/`)

1. **`CustomerHealth.cs`**: `New = 0`, `Healthy = 1`, `NeedAttention = 2`, `AtRisk = 3`, `Dormant = 4`, `Churned = 5`.

2. **`SalesTaskStatus.cs`**: Đặt tên `SalesTaskStatus` (tránh đụng độ `System.Threading.Tasks.TaskStatus`): `Pending = 0`, `InProgress = 1`, `Completed = 2`, `Skipped = 3`, `Cancelled = 4`.

3. **`TaskOutcome.cs`**: `CustomerWillBuy = 1`, `NeedQuote = 2`, `AgreeMeeting = 3`, `CustomerBusy = 4`, `NoAnswer = 5`, `AlreadyBought = 6`, `Complaint = 7`, `CareOnly = 8`.

4. **`QuoteStatus.cs`**: `Draft = 0`, `PendingApproval = 1`, `Sent = 2`, `Accepted = 3`, `Rejected = 4`, `Expired = 5`.

5. **`InteractionType.cs`**: `Call = 1`, `Meeting = 2`, `Email = 3`, `Zalo = 4`, `Visit = 5`.

### 5.3 Khởi tạo 2 Helpers cốt lõi (`src/Crm.Business/Helpers/`)

1. **`CurrencyHelper.cs`**: Hỗ trợ định dạng VND chuẩn (`Format(1065000000)` $\rightarrow$ `1.065.000.000 đ`) và dạng viết tắt trực quan trên Dashboard (`FormatShort(1065000000)` $\rightarrow$ `1.07 tỷ`).

2. **`DateHelper.cs`**: Chuyển đổi linh hoạt múi giờ Việt Nam giữa Windows (`SE Asia Standard Time`) và Linux/Docker (`Asia/Ho_Chi_Minh`).

Kiểm tra build:

```
dotnet build
# Kỳ vọng: Build succeeded. 0 Warning(s), 0 Error(s).

```

## PHASE 6 — CHẨN ĐOÁN & SỬA LỖI MODEL VALIDATION (TEAMID1)

### 6.1 Triệu chứng thực tế phát hiện trong Smoke Test

Khi chạy `dotnet run` tại `src/Crm.Web`, terminal cảnh báo:

```
warn: Microsoft.EntityFrameworkCore.Model.Validation[10625]
      The foreign key property 'UserProfile.TeamId1' was created in shadow state because a conflicting property with the simple name 'TeamId' exists in the entity type, but is either not mapped, is already used for another relationship, or is incompatible with the associated primary key type.

```

### 6.2 Phân tích nguyên nhân gốc rễ (Root Cause Analysis)

* Trong `UserProfile.cs` đã khai báo:

  ```
  public int? TeamId { get; set; }
  public virtual Team? Team { get; set; }
  
  ```

* Trong `Team.cs` đã khai báo:

  ```
  public virtual ICollection<UserProfile> UserProfiles { get; set; }
  
  ```

* Tuy nhiên trong `AppIdentityDbContext.cs`, quan hệ được cấu hình bằng dấu ngoặc rỗng:

  ```
  entity.HasOne(u => u.Team)
      .WithMany() // <-- LỖI: Để trống khiến EF Core hiểu là quan hệ 1 chiều độc lập
      .HasForeignKey(u => u.TeamId);
  
  ```

* **Hậu quả:** EF Core tưởng rằng collection `UserProfiles` trong entity `Team` là một quan hệ thứ hai khác, nên nó tự động tạo ra cột bóng ngầm **`TeamId1`** để map chiều ngược lại.

### 6.3 Giải pháp xử lý triệt để

Mở file `src/Crm.Data/AppIdentityDbContext.cs`, sửa lại cấu hình quan hệ nối đúng chiều:

```
// Quan hệ với Team
entity.HasOne(u => u.Team)
    .WithMany(t => t.UserProfiles) // Trỏ chính xác vào Navigation Property đã có
    .HasForeignKey(u => u.TeamId)
    .OnDelete(DeleteBehavior.SetNull);

```

### 6.4 Xác nhận kết quả

Khởi chạy lại Web application:

```
cd "F:\Design Web\PROJECT CRM MINI\CrmSolution\src\Crm.Web"
dotnet run

```

* **Kết quả:** Warning `TeamId1` biến mất hoàn toàn.

* Thao tác đăng nhập xác thực thành công cả 2 tài khoản:

  * `admin@crm.local` / `Admin@2026`

  * `sales1@crm.local` / `Sales@2026`

## PHASE 7 — ĐÓNG GÓI & GIT COMMIT

```
cd "F:\Design Web\PROJECT CRM MINI\CrmSolution"

# Lưu mã nguồn chính thức của Day 4
git add .
git commit -m "feat(day4): scaffold entities, seed demo data, add enums/helpers, and fix TeamId1 shadow property warning"
git push origin develop

```

## BẢNG KIỂM TRA TỔNG KẾT (FINAL AUDIT CHECKLIST)

* \[x\] Biến môi trường PostgreSQL (`PGCLIENTENCODING=UTF8`, `PGPASSWORD=sa`) hoạt động trơn tru.

* \[x\] 16 Business entities được scaffold sạch sẽ vào `Crm.Domain.Entities`.

* \[x\] Tên class `KiotVietSyncLog` đã viết hoa chữ `V`.

* \[x\] Nạp thành công và đối chiếu đủ 20 Customers, 30 Contacts, 20 Products, 15 Opportunities (1.065.000.000 đ), 45 Stage Histories.

* \[x\] Phân bổ khách hàng thành công cho `manager` (7), `sales1` (7), `sales2` (6).

* \[x\] Tạo đầy đủ 5 Enums nghiệp vụ và 2 Helpers đa nền tảng.

* \[x\] Khắc phục triệt để lỗi Shadow Property `UserProfile.TeamId1` trong `AppIdentityDbContext`.

* \[x\] Đăng nhập kiểm thử đa vai trò (Admin & Sales) đạt kết quả 100% không phát sinh runtime exception.

* \[x\] Toàn bộ mã nguồn và tài liệu đã commit và push lên nhánh `develop`.