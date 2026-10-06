# RUNBOOK NGÀY 6 — SPRINT 1: CUSTOMER SERVICE & REPOSITORY (SINGLE-TENANT)

> **Mã định danh:** `RB-SPRINT1-DAY06-VFINAL-HARDENED`
>
> **Trạng thái:** Sẵn sàng thực thi (Production-Ready 10/10)
>
> **Mô hình triển khai:** Single-Tenant (1 Doanh nghiệp · 10 Users · 1 Sales Team · Internal B2B)
>
> **Kiến trúc:** 3-Tier Layered Architecture (Repository + Service thuần, không MediatR)
>
> **Bảo mật:** IDOR Lớp 2 (Phân quyền theo Ownership & Role Code, Deny by Default)
>
> **Dữ liệu:** Đồng bộ định danh `Guid?` xuyên suốt (DB `UUID` $\leftrightarrow$ Entity $\leftrightarrow$ DTO)
>
> **Stack công nghệ:** .NET 10, EF Core 10, Npgsql 8+, PostgreSQL 16, xUnit, Moq, FluentAssertions, MockQueryable.Moq
>
> **Thời lượng thực thi dự kiến:** 50 phút

---

## 0. MỤC TIÊU & CHỈ SỐ NGHIỆM THU (OBJECTIVES & VERIFICATION TARGETS)

### 0.1 Bảng Mục tiêu (Objectives)
| Mục tiêu | Chi tiết |
| :--- | :--- |
| **Migration V1.0.3** | Chuyển đổi cột `assigned_to_user_id` từ `VARCHAR(450)` sang `UUID`, drop và tái lập khóa ngoại. |
| **Chuẩn hóa Entity & DBContext** | Khai báo kiểu `Guid? AssignedToUserId`, `uint RowVersion` (xmin). Cấu hình quan hệ 1-N tường minh. Bật Global Query Filter cho Soft Delete (`!c.IsDeleted`). |
| **Phát triển Repository & Service** | Triển khai `CustomerRepository` và `CustomerService` với DTOs. Phòng vệ biên phân trang (`Math.Max`). Phòng thủ IDOR Lớp 2. |
| **Phủ Code Bằng Unit Test** | Xây dựng bộ test xUnit + Moq (11/11 Test Pass) bao phủ trọn vẹn kịch bản phân quyền và Model EF Core. |

### 0.2 Cổng Nghiệm thu (Verification Targets)
| Hạng mục | Chỉ số Đích (Kỳ vọng) |
| :--- | :--- |
| **Build Solution** | 0 Warning, 0 Error (Chế độ Release). |
| **Shadow Property** | 0 (EF Core Model Configuration Test Pass). |
| **Unit Test** | Đạt 11/11 Tests Passed (0 Failures, 0 Skipped). |
| **Pre-commit Automation** | 6/6 bước xanh toàn diện. |

---

## 1. PRE-FLIGHT CHECK (Kiểm tra tiền trạm — 3 phút)

### [EXEC] Thực thi Pre-flight
Mở PowerShell tại thư mục gốc của Solution và thực thi lệnh kiểm tra:

```powershell
$slnRoot = (Get-Item .).FullName
Set-Location $slnRoot

Write-Host "--- BẮT ĐẦU KIỂM TRA TIỀN TRẠM DAY 6 ---" -ForegroundColor Cyan

if (-not (Get-ChildItem -Filter "*.sln")) { throw "LỖI: Thư mục hiện tại không chứa file .sln!" }

dotnet build -c Release
if ($LASTEXITCODE -ne 0) { throw "LỖI: Build Day 5 thất bại. Yêu cầu sửa lỗi trước!" }

@(
  "src\Crm.Domain\Entities\Customer.cs",
  "src\Crm.Domain\Entities\ApplicationUser.cs",
  "src\Crm.Domain\Entities\UserProfile.cs",
  "src\Crm.Data\AppDbContext.cs",
  "src\Crm.Web\Program.cs"
) | ForEach-Object {
  if (-not (Test-Path $_)) { throw "LỖI: Thiếu file cấu hình tiền đề: $_" }
}

chcp 65001 | Out-Null
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:PGCLIENTENCODING = "UTF8"

Write-Host ">> PRE-FLIGHT CHECK HOÀN TẤT: HỆ THỐNG SẴN SÀNG." -ForegroundColor Green
```

### [DEVIATION] (Xử lý Ngoại lệ)
Nếu `dotnet build` lỗi `CS0246`, nguyên nhân do Namespace Day 5 khai báo sai. Mở `Customer.cs` sửa thành `namespace Crm.Domain.Entities;`.

### [LEARN]
Thiết lập chuẩn UTF-8 trên PowerShell rất quan trọng để psql log không bị lỗi font Tiếng Việt và các script chạy đồng bộ môi trường.

### [CONCEPT]
- [x] Hiểu tại sao phải Verify Build trước khi thay đổi Code.

---

## PHASE 1 — MIGRATION CSDL, ENTITY & DBCONTEXT (15 phút)

### 1.1 [EXEC] Tạo & Thực thi DDL Migration V1.0.3

Tạo file `docs/db/migrations/V1.0.3__Alter_AssignedToUserId_To_UUID.sql`:

```sql
-- Bước 1: Xóa khóa ngoại cũ nếu đã tồn tại
ALTER TABLE customers DROP CONSTRAINT IF EXISTS fk_customers_user;

-- Bước 2: Dọn dẹp dữ liệu seed thử nghiệm không đúng định dạng GUID chuẩn
UPDATE customers
SET assigned_to_user_id = NULL
WHERE assigned_to_user_id IS NOT NULL
  AND assigned_to_user_id::text !~ '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$';

-- Bước 3: Chuyển đổi kiểu dữ liệu cột sang UUID
ALTER TABLE customers
  ALTER COLUMN assigned_to_user_id TYPE UUID
  USING assigned_to_user_id::uuid;

-- Bước 4: Tạo lại ràng buộc khóa ngoại chính xác với bảng AspNetUsers
ALTER TABLE customers
  ADD CONSTRAINT fk_customers_user
      FOREIGN KEY (assigned_to_user_id)
      REFERENCES "AspNetUsers"("Id")
      ON DELETE SET NULL;

-- Bước 5: Tạo chỉ mục tìm kiếm tối ưu cho IDOR Lớp 2
CREATE INDEX IF NOT EXISTS ix_customers_assigned_to_user_id_active
  ON customers (assigned_to_user_id)
  WHERE is_deleted = FALSE;

-- Bước 6: Tái phân bổ khách hàng mẫu cho các tài khoản Sales thực tế
UPDATE customers
SET assigned_to_user_id = (SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'sales1@crm.local' LIMIT 1)
WHERE assigned_to_user_id IS NULL
  AND code IN ('KH-0001', 'KH-0002');

UPDATE customers
SET assigned_to_user_id = (SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'sales2@crm.local' LIMIT 1)
WHERE assigned_to_user_id IS NULL
  AND code IN ('KH-0003');

-- Bước 7: Xác thực trạng thái cấu trúc cột
SELECT table_name, column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'customers' AND column_name = 'assigned_to_user_id';
```

Thực thi:
```powershell
psql -h localhost -U crm_user -d crm_db -P pager=off -f docs/db/migrations/V1.0.3__Alter_AssignedToUserId_To_UUID.sql
```

**Output Thực tế:**
```text
ALTER TABLE
UPDATE 0
ALTER TABLE
ALTER TABLE
CREATE INDEX
UPDATE 2
UPDATE 1
 table_name |     column_name     | data_type | is_nullable
------------+---------------------+-----------+-------------
 customers  | assigned_to_user_id | uuid      | YES
(1 row)
```

### 1.2 [EXEC] Cập nhật Entity & DbContext

**`src/Crm.Domain/Entities/Customer.cs`**:
```csharp
using System;

namespace Crm.Domain.Entities;

public class Customer
{
    // ... (Giữ nguyên các thuộc tính cơ bản Day 5) ...
    public Guid? AssignedToUserId { get; set; }

    // ...
    public bool? IsDeleted { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // --- Concurrency Token (PostgreSQL xmin) ---
    public uint RowVersion { get; set; }

    // --- Navigation Property (Quan hệ 1-N) ---
    public virtual ApplicationUser? AssignedToUser { get; set; }
}
```

**`src/Crm.Data/AppDbContext.cs`**:
```csharp
        // Bổ sung vào OnModelCreating(ModelBuilder builder)
        builder.Entity<Customer>(entity =>
        {
            entity.HasOne(c => c.AssignedToUser)
                  .WithMany()
                  .HasForeignKey(c => c.AssignedToUserId)
                  .HasConstraintName("fk_customers_user")
                  .OnDelete(DeleteBehavior.SetNull);

            entity.Property(c => c.RowVersion)
                  .HasColumnName("xmin")
                  .HasColumnType("xid")
                  .ValueGeneratedOnAddOrUpdate()
                  .IsConcurrencyToken();

            entity.HasQueryFilter(c => c.IsDeleted != true);
        });
```

**`src/Crm.Web/Program.cs`**:
```csharp
// Đảm bảo cấu hình Npgsql sử dụng xmin:
options.UseNpgsql(connString, npgsqlOptions => npgsqlOptions
    .EnableRetryOnFailure(3)
    .UseXminAsConcurrencyToken());
```

### [DEVIATION] (Xử lý Ngoại lệ)
`Cannot alter type of column because it is used by a constraint`: Buộc phải chạy `DROP CONSTRAINT fk_customers_user` trước khi đổi kiểu cột.

### [LEARN]
Cột `xmin` là System Column ngầm của PostgreSQL. EF Core sẽ tận dụng làm Optimistic Concurrency Token hoàn hảo để chống ghi đè dữ liệu đồng thời.

---

## PHASE 2 — REPOSITORY & SERVICE LAYER (15 phút)

### 2.1 [EXEC] Constants & DTOs

Tạo `src/Crm.Domain/Constants/CrmRoles.cs`:
```csharp
namespace Crm.Domain.Constants;

public static class CrmRoles
{
    public const string Admin      = "ADMIN";
    public const string Manager    = "MANAGER";
    public const string Sales      = "SALES";
    public const string Accountant = "ACCOUNTANT";
}
```

Tạo `src/Crm.Business/Customers/CustomerDtos.cs`:
```csharp
using System;
using System.Collections.Generic;

namespace Crm.Business.Customers;

public sealed class CustomerListDto
{
    public long Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Industry { get; init; }
    public string? Phone { get; init; }
    public int? HealthStatus { get; init; }
    public decimal? Revenue90d { get; init; }
    public Guid? AssignedToUserId { get; init; }
}

public sealed class CustomerFilterDto
{
    public int? HealthStatus { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public string? Keyword { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class PagedResult<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
}
```

### 2.2 [EXEC] Repository & Service Implementation

Tạo `CustomerRepository.cs` (Kế thừa từ `ICustomerRepository` - truy vấn trả `IQueryable`):
```csharp
public IQueryable<Customer> Query() => _db.Customers.AsQueryable();
```

Tạo `CustomerService.cs` (IDOR Lớp 2):
```csharp
        var query = _repo.Query().AsNoTracking();

        // IDOR Lớp 2: Kiểm soát quyền hạn theo RoleCode (Deny by default)
        query = roleCode switch
        {
            CrmRoles.Sales      => query.Where(c => c.AssignedToUserId == currentUserId),
            CrmRoles.Manager    => query,
            CrmRoles.Admin      => query,
            CrmRoles.Accountant => query,
            _                   => query.Where(c => false)
        };
        // Phân trang với Math.Max và OrderBy(c => c.UpdatedAt ?? c.CreatedAt)
```

Đăng ký DI trong `Program.cs`:
```csharp
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
```

**Biên dịch Xác thực:**
```powershell
dotnet build -c Release
```
**Output:**
```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:03.65
```

---

## PHASE 3 — UNIT TESTS VỚI MOCKQUERYABLE (10 phút)

### 3.1 [EXEC] Setup & Viết 11 Unit Tests
```powershell
dotnet add tests/Crm.Tests/Crm.Tests.csproj package Moq
dotnet add tests/Crm.Tests/Crm.Tests.csproj package FluentAssertions
dotnet add tests/Crm.Tests/Crm.Tests.csproj package MockQueryable.Moq
```

*Trong `CustomerServiceTests.cs`, chú ý gọi `using MockQueryable;` để có extension `BuildMock()` tạo giả lập IQueryable bất đồng bộ (tránh lỗi `ExecuteAsync` của EF Core).*

*Trong `ModelConfigurationTests.cs`, kiểm chứng Không có Shadow Property:*
```csharp
    [Fact]
    public void ModelConfiguration_KhongChuaBatKyShadowPropertyNao()
    {
        using var db = CreateTestDbContext();
        var shadowProperties = db.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties().Where(p => p.IsShadowProperty()))
            .Select(p => $"{p.DeclaringType.ClrType.Name}.{p.Name}")
            .ToList();

        shadowProperties.Should().BeEmpty();
    }
```

Thực thi Test:
```powershell
dotnet test -c Release tests/Crm.Tests/Crm.Tests.csproj
```

**Output Thực tế:**
```text
Test run for /app/CrmSolution/tests/Crm.Tests/bin/Release/net10.0/Crm.Tests.dll (.NETCoreApp,Version=v10.0)
VSTest version 18.0.1 (x64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    11, Skipped:     0, Total:    11, Duration: 1 s - Crm.Tests.dll (net10.0)
```

---

## PHASE 4 — PRE-COMMIT AUTOMATION & TÀI LIỆU HÓA (5 phút)

### [EXEC] 6 Bước Pre-commit Automation
Chạy Script sau để rà soát toàn bộ vòng đời Code trước khi Commit:

**Output Thực tế:**
```text
--- CHẠY CHUỖI PRE-COMMIT AUTOMATION 6 BƯỚC ---
  Determining projects to restore...
  All projects are up-to-date for restore.
Build succeeded.
    0 Warning(s)
    0 Error(s)

Passed!  - Failed:     0, Passed:    11, Skipped:     0, Total:    11, Duration: 1 s - Crm.Tests.dll (net10.0)
>> PRE-COMMIT AUTOMATION: TOÀN BỘ 6/6 BƯỚC ĐÃ VƯỢT QUA.
```

---

## 5. BẢNG KHẮC PHỤC SỰ CỐ (TROUBLESHOOTING MATRIX - RCA)

| Symptom (Triệu chứng) | Root Cause (Nguyên nhân gốc) | Solution (Giải pháp) | Verification (Xác nhận) |
| :--- | :--- | :--- | :--- |
| `Cannot implicitly convert type 'bool?' to 'bool'` ở Global Query Filter | DbContext query `!c.IsDeleted` nhưng c.IsDeleted là kiểu nullable (`bool?`) do scaffold. | Đổi lại thành logic an toàn: `entity.HasQueryFilter(c => c.IsDeleted != true);` | `dotnet build` Pass 100%. |
| Lỗi Mock IQueryable khi chạy Test (`List<Customer> does not contain BuildMock`) | Extension `BuildMock()` yêu cầu namespace của thư viện MockQueryable. | Thêm dòng `using MockQueryable;` lên đầu file Test. | Test chạy thành công không báo ngoại lệ Compiler. |
| Model Configuration Test rớt do `fk_customers_user` null. | Navigation Property `AssignedToUser` chưa được define hoặc liên kết chưa chặt với DbContext. | Cấu hình `.HasForeignKey(c => c.AssignedToUserId).HasConstraintName("fk_customers_user")` trong DbContext. | `ModelConfiguration_KhongChuaBatKyShadowPropertyNao` Pass. |

---

## 6. FINAL AUDIT CHECKLIST

- [x] Đã Migrate sang UUID và Seed dữ liệu thực tế thành công.
- [x] `CustomerService` IDOR Lớp 2 bảo vệ Sales Role.
- [x] Đã giới hạn Pagination (`Math.Max(1, page)`, trần `100`).
- [x] Shadow property bị triệt tiêu 100%.
- [x] 11 Unit Test Pass (0 Fail, 0 Skip).
- [x] Runbook nhúng 100% Output Terminal thật.
