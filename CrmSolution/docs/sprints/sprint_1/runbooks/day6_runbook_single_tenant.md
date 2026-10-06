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

## 0. HIỆN TRẠNG ĐẦU VÀO (DAY 1 – DAY 5)

Trước khi thực thi Day 6, hệ thống đã hoàn tất các mốc kỹ thuật từ Day 1 đến Day 5:

* **Solution cấu trúc 4 tầng:** `Crm.Domain`, `Crm.Data`, `Crm.Business`, `Crm.Web` và `tests/Crm.Tests`.
* **Database Schema:** 16 bảng nghiệp vụ PostgreSQL + ASP.NET Core Identity.
* **Tài khoản định danh:** 5 User mẫu đã được seed (`admin`, `manager`, `sales1`, `sales2`, `accountant`) với `Id` kiểu `UUID`.
* **Entities nền tảng:** Scaffolded các entity, `UserProfile` (chứa `RoleCode`, `TeamId`), `Team` (khóa chính `int`).
* **Giao diện & Tiện ích:** MudBlazor Layout, Serilog JSON File Logger, các Enum nghiệp vụ (`CustomerHealth`).

---

## 1. MỤC TIÊU & CHỈ SỐ NGHIỆM THU (DoD)

### 1.1 Mục tiêu kỹ thuật

1. **Migration V1.0.3:** Chuyển đổi cột `assigned_to_user_id` từ `VARCHAR(450)` sang `UUID`, drop và tái lập khóa ngoại trỏ tới `"AspNetUsers"("Id")`.
2. **Chuẩn hóa Entity `Customer`:** Khai báo kiểu `Guid? AssignedToUserId`, `DateTime? UpdatedAt` (an toàn dữ liệu null), `uint RowVersion` (System column `xmin`), và Navigation Property tường minh `virtual ApplicationUser? AssignedTo`.
3. **Cấu hình `AppDbContext` toàn vẹn:** Thiết lập quan hệ 1-N cho `Customer` mà không làm mất cấu hình `UserProfile` và `Team` của Day 4; bật Global Query Filter cho Soft Delete (`!c.IsDeleted`); cấu hình Concurrency Token với `xmin`.
4. **Phát triển Tầng Dữ liệu & Nghiệp vụ:** Triển khai `CustomerRepository` và `CustomerService` có phòng vệ biên phân trang (`Math.Max`, trần `PageSize`), tìm kiếm, sắp xếp kết hợp (`UpdatedAt ?? CreatedAt`), và phòng thủ IDOR Lớp 2.
5. **Kiểm thử tự động:** Xây dựng 10 bài Unit Test bao phủ trọn vẹn các kịch bản phân quyền (Sales, Manager, Admin, Accountant, Role lạ, khách hàng chưa phân công `null`) và cấu hình Model EF Core.
6. **Pre-commit Automation:** Chạy chuỗi kiểm soát chất lượng 6 bước (Format, Build, Test, Secret Scan, UI Audit, Doc Drift).

### 1.2 Cổng nghiệm thu (Definition of Done)

* [ ] Build Solution ở chế độ Release: **0 Warning, 0 Error**.
* [ ] Số lượng Shadow Property phát sinh trong DbContext: **0**.
* [ ] Độ phủ Unit Test: **10/10 Tests Passed** (8 Service Tests + 2 Model Tests).
* [ ] Rủi ro ngoại lệ runtime: **0** (Loại trừ hoàn toàn `Guid.Parse`, `CS0019` và `ArgumentOutOfRangeException` khi phân trang).

---

## 2. PRE-FLIGHT CHECK (Kiểm tra tiền trạm — 3 phút)

Mở PowerShell tại thư mục gốc của Solution và thực thi lệnh kiểm tra:

```powershell
$slnRoot = (Get-Item .).FullName
Set-Location $slnRoot

Write-Host "--- BẮT ĐẦU KIỂM TRA TIỀN TRẠM DAY 6 ---" -ForegroundColor Cyan

# 1. Kiểm tra file Solution
if (-not (Get-ChildItem -Filter "*.sln")) {
    throw "LỖI: Thư mục hiện tại không chứa file .sln!"
}

# 2. Kiểm tra biên dịch nền tảng Day 5
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    throw "LỖI: Build Day 5 thất bại. Yêu cầu sửa lỗi trước khi thực hiện Day 6!"
}

# 3. Kiểm tra các file tiền đề bắt buộc
@(
  "src\Crm.Domain\Entities\Customer.cs",
  "src\Crm.Domain\Entities\ApplicationUser.cs",
  "src\Crm.Domain\Entities\UserProfile.cs",
  "src\Crm.Domain\Enums\CustomerHealth.cs",
  "src\Crm.Data\AppDbContext.cs",
  "src\Crm.Web\Program.cs"
) | ForEach-Object {
  if (-not (Test-Path $_)) { throw "LỖI: Thiếu file cấu hình tiền đề: $_" }
}

# 4. Thiết lập chuẩn Encoding UTF-8 cho kết nối CLI
chcp 65001 | Out-Null
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:PGCLIENTENCODING = "UTF8"

Write-Host ">> PRE-FLIGHT CHECK HOÀN TẤT: HỆ THỐNG SẴN SÀNG." -ForegroundColor Green
```

---

## PHASE 1 — MIGRATION CSDL, ENTITY & DBCONTEXT (15 phút)

### 1.1 Tạo File DDL Migration V1.0.3

Tạo file `docs/db/migrations/V1.0.3__Alter_AssignedToUserId_To_UUID.sql` với nội dung chuẩn hóa kiểu dữ liệu:

```sql
-- ============================================================================
-- Migration: V1.0.3__Alter_AssignedToUserId_To_UUID.sql
-- Mục tiêu: Chuẩn hóa assigned_to_user_id sang UUID, đồng bộ với AspNetUsers.Id
-- Tính chất: Idempotent, có kiểm soát ràng buộc toàn vẹn
-- ============================================================================

-- Bước 1: Xóa khóa ngoại cũ nếu đã tồn tại
ALTER TABLE customers DROP CONSTRAINT IF EXISTS fk_customers_user;

-- Bước 2: Dọn dẹp dữ liệu seed thử nghiệm không đúng định dạng GUID chuẩn
UPDATE customers
SET assigned_to_user_id = NULL
WHERE assigned_to_user_id IS NOT NULL
  AND assigned_to_user_id !~ '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$';

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

-- Bản ghi KH-0004 giữ assigned_to_user_id = NULL để kiểm thử trường hợp chưa phân công

-- Bước 7: Xác thực trạng thái cấu trúc cột
SELECT table_name, column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'customers' AND column_name = 'assigned_to_user_id';
```

Thực thi file script vào cơ sở dữ liệu PostgreSQL:

```powershell
psql -h localhost -U crm_user -d crm_db -P pager=off -f docs/db/migrations/V1.0.3__Alter_AssignedToUserId_To_UUID.sql
```

### 1.2 Cập nhật Entity `Customer.cs`

Mở file `src/Crm.Domain/Entities/Customer.cs` và cập nhật các thuộc tính:

```csharp
using System;

namespace Crm.Domain.Entities;

public class Customer
{
    // --- Các trường dữ liệu cơ bản ---
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Industry { get; set; }
    public string? TaxCode { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    // --- Phân công & Sở hữu (Chuẩn UUID/Guid?) ---
    public Guid? AssignedToUserId { get; set; }

    // --- Chỉ số bán hàng & Theo dõi tương tác ---
    public int HealthStatus { get; set; }
    public int AverageCycleDays { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public DateTime? LastContactDate { get; set; }
    public DateTime? NextContactDue { get; set; }
    public decimal Revenue90d { get; set; }
    public int OrderCount90d { get; set; }
    public decimal CurrentDebt { get; set; }
    public long? KiotvietId { get; set; }
    public string? Note { get; set; }

    // --- Trạng thái vòng đời & Audit trail ---
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } // Nullable an toàn cho bản ghi mới

    // --- Concurrency Token (PostgreSQL xmin) ---
    public uint RowVersion { get; set; }

    // --- Navigation Property (Quan hệ 1-N) ---
    public virtual ApplicationUser? AssignedTo { get; set; }
}
```

### 1.3 Cấu hình Toàn vẹn trong `AppDbContext.cs`

Mở file `src/Crm.Data/AppDbContext.cs`. Đảm bảo cấu hình tường minh khóa ngoại cho `Customer`, đồng thời **giữ nguyên vẹn cấu hình của `UserProfile` và `Team` từ Day 3 và Day 4**:

```csharp
using Crm.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Crm.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Team> Teams => Set<Team>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ====================================================================
        // 1. CẤU HÌNH DAY 3 & DAY 4 (BẢO LƯU TOÀN VẸN - KHÔNG XÓA)
        // ====================================================================
        builder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(p => p.UserId);

            entity.HasOne(p => p.User)
                  .WithOne()
                  .HasForeignKey<UserProfile>(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Team)
                  .WithMany(t => t.Members)
                  .HasForeignKey(p => p.TeamId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Team>(entity =>
        {
            entity.ToTable("teams");
            entity.HasKey(t => t.Id);
        });

        // ====================================================================
        // 2. CẤU HÌNH DAY 6: CUSTOMER & CONCURRENCY
        // ====================================================================
        builder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");

            // Quan hệ tường minh: ApplicationUser không có Collection ngược -> .WithMany() không tham số
            entity.HasOne(c => c.AssignedTo)
                  .WithMany()
                  .HasForeignKey(c => c.AssignedToUserId)
                  .HasConstraintName("fk_customers_user")
                  .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình xmin làm Concurrency Token
            entity.Property(c => c.RowVersion)
                  .HasColumnName("xmin")
                  .HasColumnType("xid")
                  .ValueGeneratedOnAddOrUpdate()
                  .IsConcurrencyToken();

            // Global Query Filter: Tự động loại trừ các bản ghi xóa mềm
            entity.HasQueryFilter(c => !c.IsDeleted);
        });
    }
}
```

### 1.4 Kích hoạt Token trong `Program.cs`

Mở file `src/Crm.Web/Program.cs`, tìm đoạn đăng ký DbContext và bổ sung cấu hình `.UseXminAsConcurrencyToken()`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions
            .EnableRetryOnFailure(3)
            .UseXminAsConcurrencyToken()));
```

---

## PHASE 2 — REPOSITORY & SERVICE LAYER (15 phút)

### 2.1 Định nghĩa Constants Phân quyền

Tạo file `src/Crm.Domain/Constants/CrmRoles.cs`:

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

### 2.2 Tầng Repository (Data Access)

Tạo interface `src/Crm.Data/Repositories/ICustomerRepository.cs`:

```csharp
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Domain.Entities;

namespace Crm.Data.Repositories;

public interface ICustomerRepository
{
    IQueryable<Customer> Query();
    Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
    void Update(Customer customer);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

Tạo lớp triển khai `src/Crm.Data/Repositories/CustomerRepository.cs`:

```csharp
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Data.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db)
    {
        _db = db;
    }

    public IQueryable<Customer> Query() => _db.Customers.AsQueryable();

    public Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default)
        => _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
        => await _db.Customers.AddAsync(customer, ct);

    public void Update(Customer customer) => _db.Customers.Update(customer);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
```

### 2.3 Các DTOs Truy vấn & Phân trang

Tạo file `src/Crm.Business/Customers/CustomerDtos.cs`:

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
    public int HealthStatus { get; init; }
    public decimal Revenue90d { get; init; }
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

### 2.4 Tầng Service với IDOR Lớp 2 & Phòng ngự Phân trang

Tạo interface `src/Crm.Business/Customers/ICustomerService.cs`:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crm.Business.Customers;

public interface ICustomerService
{
    Task<PagedResult<CustomerListDto>> GetListAsync(
        CustomerFilterDto filter,
        Guid currentUserId,
        string roleCode,
        CancellationToken ct = default);
}
```

Tạo lớp triển khai `src/Crm.Business/Customers/CustomerService.cs`:

```csharp
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Data.Repositories;
using Crm.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace Crm.Business.Customers;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<CustomerListDto>> GetListAsync(
        CustomerFilterDto f,
        Guid currentUserId,
        string roleCode,
        CancellationToken ct = default)
    {
        // 1. Chốt chặn phòng thủ phân trang (Pagination Boundary Defense)
        var pageIndex = Math.Max(1, f.PageIndex);
        var pageSize = f.PageSize switch
        {
            < 1   => 20,
            > 100 => 100, // Chống khai thác cạn kiệt tài nguyên bộ nhớ
            _     => f.PageSize
        };

        // 2. Tối ưu bộ nhớ cho tác vụ chỉ đọc
        var query = _repo.Query().AsNoTracking();

        // 3. IDOR Lớp 2: Kiểm soát quyền hạn theo RoleCode (Deny by default)
        query = roleCode switch
        {
            CrmRoles.Sales      => query.Where(c => c.AssignedToUserId == currentUserId),
            CrmRoles.Manager    => query, // 1 Team chung -> Manager giám sát toàn bộ
            CrmRoles.Admin      => query, // Quản trị viên hệ thống có toàn quyền
            CrmRoles.Accountant => query, // Kế toán đối soát công nợ toàn công ty
            _                   => query.Where(c => false) // Role không hợp lệ -> Chặn tuyệt đối
        };

        // 4. Áp dụng các bộ lọc nghiệp vụ
        if (f.HealthStatus.HasValue)
        {
            query = query.Where(c => c.HealthStatus == f.HealthStatus.Value);
        }

        if (f.AssignedToUserId.HasValue)
        {
            query = query.Where(c => c.AssignedToUserId == f.AssignedToUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(f.Keyword))
        {
            var kw = f.Keyword.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(kw) ||
                c.Code.ToLower().Contains(kw) ||
                (c.Phone != null && c.Phone.Contains(kw)));
        }

        // 5. Đếm tổng số bản ghi thỏa điều kiện
        var total = await query.CountAsync(ct);

        // 6. Phân trang và sắp xếp có fallback (Ưu tiên UpdatedAt, dự phòng CreatedAt)
        var items = await query
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerListDto
            {
                Id               = c.Id,
                Code             = c.Code,
                Name             = c.Name,
                Industry         = c.Industry,
                Phone            = c.Phone,
                HealthStatus     = c.HealthStatus,
                Revenue90d       = c.Revenue90d,
                AssignedToUserId = c.AssignedToUserId
            })
            .ToListAsync(ct);

        return new PagedResult<CustomerListDto>
        {
            Items      = items,
            TotalCount = total,
            PageIndex  = pageIndex,
            PageSize   = pageSize
        };
    }
}
```

### 2.5 Đăng ký Dependency Injection

Mở file `src/Crm.Web/Program.cs` và đăng ký các dịch vụ mới:

```csharp
using Crm.Business.Customers;
using Crm.Data.Repositories;

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
```

---

## PHASE 3 — BỘ KIỂM THỬ TỰ ĐỘNG (UNIT TESTS — 12 phút)

### 3.1 Cài đặt Package Kiểm thử

Chạy các lệnh bổ sung thư viện hỗ trợ mock truy vấn IQueryable:

```powershell
dotnet add tests/Crm.Tests/Crm.Tests.csproj package Moq --version 4.20.72
dotnet add tests/Crm.Tests/Crm.Tests.csproj package FluentAssertions --version 6.12.2
dotnet add tests/Crm.Tests/Crm.Tests.csproj package MockQueryable.Moq --version 7.0.3
dotnet add tests/Crm.Tests/Crm.Tests.csproj reference src/Crm.Business/Crm.Business.csproj
dotnet add tests/Crm.Tests/Crm.Tests.csproj reference src/Crm.Data/Crm.Data.csproj
```

### 3.2 Unit Tests cho `CustomerService` (8 Test Cases)

Tạo file `tests/Crm.Tests/Customers/CustomerServiceTests.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Business.Customers;
using Crm.Data.Repositories;
using Crm.Domain.Constants;
using Crm.Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Crm.Tests.Customers;

public class CustomerServiceTests
{
    private static readonly Guid SalesUser1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SalesUser2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static CustomerService BuildServiceFixture()
    {
        var mockData = new List<Customer>
        {
            new() { 
                Id = 1, Code = "KH-0001", Name = "Công ty An Phát", 
                AssignedToUserId = SalesUser1, HealthStatus = 1, 
                IsDeleted = false, CreatedAt = DateTime.UtcNow, UpdatedAt = null 
            },
            new() { 
                Id = 2, Code = "KH-0002", Name = "Tập đoàn Việt Nhật", 
                AssignedToUserId = SalesUser1, HealthStatus = 1, 
                IsDeleted = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow 
            },
            new() { 
                Id = 3, Code = "KH-0003", Name = "Nhựa Tân Á", 
                AssignedToUserId = SalesUser2, HealthStatus = 2, 
                IsDeleted = false, CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow 
            },
            new() { 
                Id = 4, Code = "KH-0004", Name = "Cơ khí Nam Hà (Chưa phân công)", 
                AssignedToUserId = null, HealthStatus = 0, 
                IsDeleted = false, CreatedAt = DateTime.UtcNow.AddDays(-2), UpdatedAt = null 
            }
        }.BuildMock();

        var repoMock = new Mock<ICustomerRepository>();
        repoMock.Setup(r => r.Query()).Returns(mockData);

        return new CustomerService(repoMock.Object);
    }

    [Fact]
    public async Task GetListAsync_RoleSales_ChiTraVeKhachHangCuaChinhMinh_KhongBaoGomKhachChuaGan()
    {
        var sut = BuildServiceFixture();
        var result = await sut.GetListAsync(new CustomerFilterDto(), SalesUser1, CrmRoles.Sales);

        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(c => c.AssignedToUserId == SalesUser1);
    }

    [Fact]
    public async Task GetListAsync_RoleManager_TraVeToanBoKhachHangBaoGomKhachChuaGan()
    {
        var sut = BuildServiceFixture();
        var result = await sut.GetListAsync(new CustomerFilterDto(), Guid.NewGuid(), CrmRoles.Manager);

        result.TotalCount.Should().Be(4);
    }

    [Fact]
    public async Task GetListAsync_RoleAdmin_TraVeToanBoKhachHang()
    {
        var sut = BuildServiceFixture();
        var result = await sut.GetListAsync(new CustomerFilterDto(), Guid.NewGuid(), CrmRoles.Admin);

        result.TotalCount.Should().Be(4);
    }

    [Fact]
    public async Task GetListAsync_RoleAccountant_TraVeToanBoKhachHangDeDoiSoat()
    {
        var sut = BuildServiceFixture();
        var result = await sut.GetListAsync(new CustomerFilterDto(), Guid.NewGuid(), CrmRoles.Accountant);

        result.TotalCount.Should().Be(4);
    }

    [Fact]
    public async Task GetListAsync_RoleLaHoacKhongHopLe_DenyByDefaultTraVeRong()
    {
        var sut = BuildServiceFixture();
        var result = await sut.GetListAsync(new CustomerFilterDto(), Guid.NewGuid(), "UNKNOWN_ROLE");

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetListAsync_LocTheoHealthStatus_ChinhXac()
    {
        var sut = BuildServiceFixture();
        var filter = new CustomerFilterDto { HealthStatus = 2 };
        var result = await sut.GetListAsync(filter, Guid.NewGuid(), CrmRoles.Admin);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Nhựa Tân Á");
    }

    [Fact]
    public async Task GetListAsync_LocTheoAssignedToUserId_ChinhXac()
    {
        var sut = BuildServiceFixture();
        var filter = new CustomerFilterDto { AssignedToUserId = SalesUser2 };
        var result = await sut.GetListAsync(filter, Guid.NewGuid(), CrmRoles.Admin);

        result.Items.Should().HaveCount(1);
        result.Items[0].Code.Should().Be("KH-0003");
    }

    [Fact]
    public async Task GetListAsync_TimKiemTheoTuKhoa_ChinhXac()
    {
        var sut = BuildServiceFixture();
        var filter = new CustomerFilterDto { Keyword = "phát" };
        var result = await sut.GetListAsync(filter, Guid.NewGuid(), CrmRoles.Admin);

        result.Items.Should().HaveCount(1);
        result.Items[0].Code.Should().Be("KH-0001");
    }
}
```

### 3.3 Unit Tests cho Cấu hình EF Model (2 Test Cases)

Tạo file `tests/Crm.Tests/Data/ModelConfigurationTests.cs`:

```csharp
using System;
using System.Linq;
using Crm.Data;
using Crm.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Crm.Tests.Data;

public class ModelConfigurationTests
{
    private static AppDbContext CreateTestDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=test_model_only;Username=test;Password=test")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void ModelConfiguration_KhongChuaBatKyShadowPropertyNao()
    {
        using var db = CreateTestDbContext();
        var shadowProperties = db.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties().Where(p => p.IsShadowProperty()))
            .Select(p => $"{p.DeclaringType.ClrType.Name}.{p.Name}")
            .ToList();

        shadowProperties.Should().BeEmpty("EF Core không được tự động sinh shadow property ngoài ý muốn");
    }

    [Fact]
    public void Customer_AssignedToRelationship_DuocCauHinhTuongMinh()
    {
        using var db = CreateTestDbContext();
        var foreignKey = db.Model.FindEntityType(typeof(Customer))!
            .GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(ApplicationUser));

        foreignKey.Should().NotBeNull();
        foreignKey!.Properties.Should().ContainSingle(p => p.Name == "AssignedToUserId");
        foreignKey.GetConstraintName().Should().Be("fk_customers_user");
        foreignKey.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);
    }
}
```

---

## PHASE 4 — PRE-COMMIT AUTOMATION & TÀI LIỆU HÓA (5 phút)

### 4.1 Chuỗi Pre-Commit 6 Bước Tự Động

Thực thi đoạn script PowerShell sau tại thư mục gốc để kiểm định toàn diện chất lượng:

```powershell
Set-Location $slnRoot
Write-Host "--- CHẠY CHUỖI PRE-COMMIT AUTOMATION 6 BƯỚC ---" -ForegroundColor Cyan

# Bước 1: Định dạng mã nguồn
dotnet format --verify-no-changes
if ($LASTEXITCODE -ne 0) { throw "LỖI BƯỚC 1: Code chưa được format chuẩn." }

# Bước 2: Build Release
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) { throw "LỖI BƯỚC 2: Biên dịch Release thất bại." }

# Bước 3: Chạy toàn bộ Unit Tests
dotnet test --configuration Release --no-build
if ($LASTEXITCODE -ne 0) { throw "LỖI BƯỚC 3: Có Unit Test bị Failed." }

# Bước 4: Quét chuỗi nhạy cảm (Secret Scan)
$secretPatterns = @('password\s*=\s*"[^"]+"', 'apikey\s*=\s*"[^"]+"', 'secret\s*=\s*"[^"]+"')
$violations = @()
Get-ChildItem -Recurse -Include *.cs,*.json,*.sql |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    ForEach-Object {
        foreach ($p in $secretPatterns) {
            if (Select-String -Path $_.FullName -Pattern $p -Quiet) {
                $violations += $_.FullName; break
            }
        }
    }
if ($violations.Count -gt 0) { throw "LỖI BƯỚC 4: Phát hiện nguy cơ lộ mật khẩu tại: $($violations -join ', ')" }

# Bước 5: Kiểm tra tuân thủ UI Razor
$uiViolations = Get-ChildItem -Recurse -Include *.razor -ErrorAction SilentlyContinue |
    Select-String -Pattern 'style\s*=\s*"[^@][^"]*"' -List
if ($uiViolations) { throw "LỖI BƯỚC 5: Phát hiện hard-code inline CSS trong file .razor" }

# Bước 6: Kiểm tra tính đồng bộ tài liệu (Doc Drift)
if (-not (Test-Path "docs/specs/data_dictionary.md")) { throw "LỖI BƯỚC 6: Thiếu file data_dictionary.md" }

Write-Host ">> PRE-COMMIT AUTOMATION: TOÀN BỘ 6/6 BƯỚC ĐÃ VƯỢT QUA." -ForegroundColor Green
```

### 4.2 Cập nhật Data Dictionary

Mở và ghi bổ sung vào file `docs/specs/data_dictionary.md`:

```markdown
## customers — Bản cập nhật Migration V1.0.3 (2026-10-06)

| Tên Cột | Kiểu Cũ | Kiểu Mới | Trạng Thái | Mô Tả & Ràng Buộc |
| :--- | :--- | :--- | :--- | :--- |
| `assigned_to_user_id` | `VARCHAR(450)` | `UUID` | Nullable | Khóa ngoại trỏ tới `"AspNetUsers"("Id")`, `ON DELETE SET NULL`. |
| `xmin` | — | `xid` | System Column | Cột hệ thống PostgreSQL, EF Core ánh xạ qua thuộc tính `RowVersion`. |
| `is_deleted` | `BOOLEAN` | `BOOLEAN` | Not Null | Cờ xóa mềm, tích hợp trong Global Query Filter. |
| `updated_at` | `TIMESTAMPTZ` | `TIMESTAMPTZ` | Nullable | Thời điểm cập nhật cuối cùng (Null khi mới tạo). |
```

### 4.3 Đăng ký Nợ Kỹ Thuật (Tech Debt)

Ghi nhận vào file `docs/sprints/sprint_1/tech_debt_report.md`:

```markdown
## Nợ Kỹ Thuật Ghi Nhận Tại Day 6

| # | Hạng Mục | Lý Do Tạm Thời | Giải Pháp Hoàn Chỉnh | Hạn Định |
| :-: | :--- | :--- | :--- | :-: |
| 1 | `CustomerService` mới có `GetListAsync` | Tập trung kiểm thử đọc dữ liệu và IDOR | Xây dựng tiếp `CreateAsync` và `UpdateAsync` tại Day 7 | Sprint 1 (Day 7) |
| 2 | Truyền `currentUserId` và `roleCode` tường minh | Chưa cấu hình tầng giải mã Claim Middleware | Tạo Scoped `ICurrentUserService` tự động trích xuất thông tin người dùng | Sprint 2 |
| 3 | Chưa tích hợp TestContainers | Giới hạn thời lượng triển khai Day 6 | Cấu hình Integration Tests chạy PostgreSQL Container | Sprint 2 |
```

---

## 5. BẢNG TRA CỨU & XỬ LÝ LỖI NHANH (TROUBLESHOOTING MATRIX)

| Mã Lỗi / Hiện Tượng | Nguyên Nhân Cốt Lõi | Hành Động Khắc Phục Chuẩn Xác |
| :--- | :--- | :--- |
| `CS0019: Operator '==' cannot be applied to 'Guid?' and 'string'` | Cột trong Entity `Customer` chưa được đổi sang `Guid?`, vẫn để kiểu `string?`. | Sửa khai báo tại `Customer.cs` thành `public Guid? AssignedToUserId { get; set; }`. |
| `InvalidCastException: Column contains NULL data` tại `UpdatedAt` | Thuộc tính `UpdatedAt` khai báo `DateTime` thay vì `DateTime?` trong khi DB chứa giá trị `NULL`. | Đổi thành `public DateTime? UpdatedAt { get; set; }`. |
| `ArgumentOutOfRangeException: The number of elements to skip must not be negative` | Phía client truyền `PageIndex = 0` hoặc số âm. | Dùng hàm `Math.Max(1, f.PageIndex)` trước khi thực hiện tính toán `.Skip()`. |
| Shadow Property `AssignedToUserId1` xuất hiện | Khai báo navigation nhưng thiếu chỉ định `.HasForeignKey(c => c.AssignedToUserId)`. | Bổ sung phương thức `.HasForeignKey()` vào Fluent API trong `AppDbContext`. |
| `DbUpdateConcurrencyException` không ném khi có xung đột | Quên cấu hình extension Npgsql cho cột xmin. | Đảm bảo dòng `npgsqlOptions.UseXminAsConcurrencyToken()` đã có trong `Program.cs`. |
| Lỗi migration: `cannot alter type of column because it is used by a constraint` | Cố gắng chạy lệnh `ALTER COLUMN` khi khóa ngoại đang gắn kết. | Bắt buộc chạy lệnh `DROP CONSTRAINT fk_customers_user` trước khi `ALTER COLUMN`. |

---

## 6. LỆNH COMMIT CHUẨN HÓA

Sau khi hoàn thành và vượt qua chuỗi Pre-commit, thực hiện commit mã nguồn:

```powershell
git checkout -b feature/day6-customer-service-vfinal
git add .
git commit -m "feat(customer): hoan thien service, repository va migrate assigned_to_user_id sang UUID

- Ban hanh Migration V1.0.3 chuan hoa assigned_to_user_id sang kieu UUID
- Dong bo kieu Guid? xuyen suot tu Entity, DTO den Service Query
- Khac phuc triet de nguy co loi null voi DateTime? UpdatedAt va sap xep fallback
- Bao ve phan trang voi Math.Max va gioi han PageSize chong can kiet bo nho
- Giu nguyen toan ven cau hinh Fluent API cua UserProfile va Team tu Day 4
- Trien khai IDOR Lop 2 (Role-based & Ownership) voi nguyen tac Deny by default
- Bao phu 10/10 Unit Tests (8 Service Tests gom ca case null + 2 Model Tests)
- Vuot qua 6/6 buoc kiem tra Pre-commit automation"
```