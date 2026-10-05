# RUNBOOK NGÀY 6 — SPRINT 1: CUSTOMER CQRS + IDOR 2 LỚP + MODEL TƯỜNG MINH

> **Bản:** v3.3 — Enterprise Runbook Cấp 2 (Chuẩn hóa toàn diện 78 Kỹ năng AGENTS.md)
>
> **File đích:** `docs/sprints/sprint_1/runbooks/day_6_customer_cqrs.md`
>
> **Hồ sơ kiến trúc liên kết:** `docs/adr/ADR-0002_bidirectional_navigation_and_tenant_context.md` ($[K09]$)
>
> **Thời lượng thực thi kỹ sư (K62):** 100 phút (Human Time Budget đã cộng $20\%$ biên độ an toàn)
>
> **Skills áp dụng:** K01, K02, K03, K05, K06, K07, K08, K09, K11, K12, K14, K15, K16, K17, K18, K19, K20, K21, K25, K36, K37, K39, K40, K41, K45, K46, K48, K49, K50, K51, K52, K53, K54, K58, K59, K60, K61, K62, K63, K64, K65, K66, K69, K70, K71, K74, K78

## 0. MỤC TIÊU & CHỈ SỐ NGHIỆM THU ĐỊNH LƯỢNG (OBJECTIVES & VERIFICATION TARGETS — K59, K06)

### 0.1 Bảng Mục tiêu Công việc (Objectives)

| \# | Mục tiêu Cụ thể | Kỹ năng Tham chiếu | Trạng thái Nghiệm thu |
| :---: | :--- | :--- | :---: |
| 1 | Bổ sung 8 trường quản trị CRM (`tenant_id`, `owner_id`, `team_id`, `is_deleted`, `deleted_at`, `deleted_by`, `updated_at`, `updated_by`) và tích hợp Concurrency Token qua PostgreSQL `xmin` | $[K14]$, $[K19]$, $[K20]$, $[K21]$, $[K39]$, $[K74]$ | Target |
| 2 | Khởi tạo bảng và thực thể `Team`, `User` với điều hướng 2 chiều tường minh, triệt tiêu $100\%$ shadow property ngầm | $[K16]$, $[K17]$, $[K74]$ | Target |
| 3 | Tái cấu trúc Clean Architecture: chuyển `ITenantProvider` và `ICurrentUser` vào `Crm.Domain` để triệt tiêu lỗi phụ thuộc vòng giữa `Crm.Data` và `Crm.Business` | $[K03]$, $[K12]$, $[K71]$ | Target |
| 4 | Cấu hình quan hệ tường minh qua Fluent API với 4 ràng buộc ngoại có định danh tường minh (`.HasConstraintName()`) | $[K16]$, $[K17]$ | Target |
| 5 | Triển khai CQRS Handlers qua MediatR 12: `GetCustomerListQuery` (chỉ đọc với `.AsNoTracking()`), `CreateCustomerCommand`, `UpdateCustomerCommand` | $[K12]$, $[K18]$, $[K37]$ | Target |
| 6 | Phòng thủ IDOR 2 lớp: Lớp 1 (EF Core Global Query Filter) + Lớp 2 (Handler role ownership check với nguyên tắc Deny by default) | $[K39]$ | Target |
| 7 | Thiết lập $19/19$ Unit Tests đạt tỷ lệ thành công $100\%$ (7 Query + 3 Create + 4 Update + 5 EF Model Configuration) | $[K45]$, $[K63]$ | Target |
| 8 | Thực thi Pre-commit chain 6 bước tự động kiểm tra code style, build, test, secret scan, UI audit, doc drift scan | $[K48]$, $[K49]$ | Target |
| 9 | Đồng bộ toàn diện Data Dictionary (`data_dictionary.md`) và API Contracts (`api_contracts.md`) | $[K15]$, $[K49]$ | Target |
| 10 | Ghi nhận Tech Debt, Concepts Mastered và Nhật ký vận hành 3 ngày | $[K07]$, $[K69]$, $[K70]$ | Target |

### 0.2 Cổng Nghiệm thu Định lượng (Verification Targets — K59)

| Chỉ số / Hạng mục Nghiệm thu | Ngưỡng Tối thiểu | Giá trị Đích Day 6 | Phương pháp Kiểm định |
| :--- | :--- | :--- | :--- |
| Số file mã nguồn & schema CSDL | $\ge 18$ files | **19 files** | `git status --short` |
| Số file kiểm thử unit test | $\ge 4$ files | **4 files** | `Get-ChildItem tests/Crm.Tests` |
| Số file đặc tả & ghi nhận vận hành | $\ge 5$ files | **5 files** | `docs/specs`, `docs/notes`, `docs/sprints` |
| Tổng file tạo mới hoặc cập nhật | $\ge 27$ files | **28 files** | `git status --short` |
| Kết quả Unit Test | $\ge 15$ tests pass | **19/19 tests PASS (0 fail, 0 skip)** | `dotnet test -c Release` |
| Chất lượng biên dịch Solution | 0 error | **0 Warning, 0 Error** | `dotnet build -c Release` |
| Thuộc tính ảo (Shadow Property) | 0 property | **0 shadow property** | `ModelConfigurationTests.Model_KhongCoShadowProperty` |
| Ràng buộc khóa ngoại có tên tường minh | 4 constraints | **4 constraints** | Truy vấn bảng `pg_constraint` |
| Tính khả lặp DDL (Idempotency) | Chạy 2 lần | **Không phát sinh lỗi** | Chạy script SQL 2 lần liên tiếp |
| Chuỗi chốt chặn Pre-commit | 6/6 bước | **6/6 bước PASS hoàn toàn** | Script tự động tại Phase 6 |
| Tiêu chuẩn hoàn tất (DoD) | 6 tiêu chí | **6/6 tiêu chí ĐẠT** | Final Audit Checklist |

---

## 1. PRE-FLIGHT CHECK (CHỐT CHẶN NGỮ CẢNH BAN ĐẦU — K02, K25, K51, K53)

**Human Time Budget:** 5 phút

### 1.1 [EXEC] Kịch bản kiểm tra môi trường tiền điều kiện

```powershell
# [K53] Neo thư mục gốc Solution
$slnRoot = (Get-Item .).FullName
Set-Location $slnRoot

if (-not (Get-ChildItem -Filter "*.sln")) {
    throw "[LỖI K53] Thao tác ngoài thư mục gốc Solution. Hủy thực thi."
}

# [K51] Chốt chặn 1: Build Day 5 phải xanh hoàn toàn
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    throw "[LỖI K51] Build Day 5 thất bại. Cần khắc phục lỗi biên dịch trước khi bắt đầu Day 6."
}

# [K51] Chốt chặn 2: Kiểm tra các file nền tảng Day 5
@(
    "src\Crm.Domain\Entities\Customer.cs",
    "src\Crm.Domain\Enums\CustomerHealth.cs",
    "src\Crm.Data\AppDbContext.cs"
) | ForEach-Object {
    if (-not (Test-Path (Join-Path $slnRoot $_))) {
        throw "[LỖI K51] Thiếu file phụ thuộc cốt lõi: $_"
    }
}

# [K25] Cấu hình mã hóa UTF-8 cho Console và psql
chcp 65001 | Out-Null
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:PGCLIENTENCODING = "UTF8"

Write-Host ">> PRE-FLIGHT CHECK: 100% OK — BẮT ĐẦU DAY 6" -ForegroundColor Green
```

### 1.2 [DEVIATION] Cảnh báo lệch chuẩn Pre-flight (K65)

> ⚠ **[LỆCH CHUẨN DỰ KIẾN]: `dotnet build` báo lỗi `CS0246` không tìm thấy namespace hoặc thực thể `Customer`.**
>
> * **Nguyên nhân gốc:** Mã nguồn Day 5 chưa khai báo đúng namespace `Crm.Domain.Entities` hoặc file `Customer.cs` bị đặt sai thư mục.
> * **Khắc phục nhanh:** Kiểm tra dòng đầu tiên trong `src/Crm.Domain/Entities/Customer.cs`, phải có `namespace Crm.Domain.Entities;`.

---

## PHASE 1 — DDL SCHEMA + ENTITY DOMAIN + NAVIGATION TƯỜNG MINH (K14, K16, K17, K74)

**Human Time Budget:** 25 phút

### 1.1 [SETUP] Kiểm tra kết nối CSDL PostgreSQL (K25, K52)

Trước khi thực thi script, hãy đảm bảo service PostgreSQL đang hoạt động và cổng 5432 sẵn sàng tiếp nhận kết nối. Lệnh kiểm tra này giúp bạn phát hiện sớm các vấn đề về firewall hoặc service chưa khởi động:

```powershell
Test-NetConnection -ComputerName localhost -Port 5432
```

### 1.2 [EXEC] Tạo tập tin DDL V1.0.2 Idempotent (K14, K53)

**Đường dẫn file:** `docs/specs/ddl/V1.0.2__Add_CRM_Entities.sql`

```sql
-- ============================================================================
-- V1.0.2: Bổ sung bảng teams, users và các cột CRM cho bảng customers
-- Tính chất: Idempotent — Có thể thực thi an toàn nhiều lần liên tiếp (K14, K53)
-- ============================================================================

CREATE TABLE IF NOT EXISTS sys_schema_history (
    installed_rank SERIAL PRIMARY KEY,
    version VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(200) NOT NULL,
    installed_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- ----------------------------------------------------------------------------
-- 1. Bảng teams: Cấu trúc tổ chức phòng ban và đội nhóm kinh doanh (K16, K74)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS teams (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID NOT NULL DEFAULT '00000000-0000-0000-0000-000000000001',
    name            VARCHAR(200) NOT NULL,
    parent_team_id  UUID NULL,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_teams_parent_team_id
        FOREIGN KEY (parent_team_id) REFERENCES teams(id) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ix_teams_tenant_parent
    ON teams (tenant_id, parent_team_id) WHERE is_deleted = FALSE;

-- ----------------------------------------------------------------------------
-- 2. Bảng users: Nhân sự kinh doanh và quản trị hệ thống (K16, K74)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS users (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID NOT NULL DEFAULT '00000000-0000-0000-0000-000000000001',
    team_id         UUID NULL,
    email           VARCHAR(200) NOT NULL,
    role            VARCHAR(20) NOT NULL DEFAULT 'SALES',
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_users_email UNIQUE (email),
    CONSTRAINT fk_users_team_id
        FOREIGN KEY (team_id) REFERENCES teams(id) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ix_users_tenant_team
    ON users (tenant_id, team_id) WHERE is_deleted = FALSE;

-- ----------------------------------------------------------------------------
-- 3. Bổ sung các cột CRM quản trị cho bảng customers (K20, K21, K39)
-- Ghi chú: Cột 'xmin' là system column sẵn có trong PostgreSQL, không dùng ADD COLUMN
-- ----------------------------------------------------------------------------
ALTER TABLE customers
  ADD COLUMN IF NOT EXISTS tenant_id    UUID         NOT NULL DEFAULT '00000000-0000-0000-0000-000000000001',
  ADD COLUMN IF NOT EXISTS owner_id     UUID         NULL,
  ADD COLUMN IF NOT EXISTS team_id      UUID         NULL,
  ADD COLUMN IF NOT EXISTS is_deleted   BOOLEAN      NOT NULL DEFAULT FALSE,
  ADD COLUMN IF NOT EXISTS deleted_at   TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by   UUID         NULL,
  ADD COLUMN IF NOT EXISTS updated_at   TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS updated_by   UUID         NULL;

-- ----------------------------------------------------------------------------
-- 4. Bổ sung các ràng buộc khóa ngoại tường minh cho bảng customers (K16)
-- ----------------------------------------------------------------------------
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_customers_team_id') THEN
        ALTER TABLE customers
            ADD CONSTRAINT fk_customers_team_id
                FOREIGN KEY (team_id) REFERENCES teams(id) ON DELETE RESTRICT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_customers_owner_id') THEN
        ALTER TABLE customers
            ADD CONSTRAINT fk_customers_owner_id
                FOREIGN KEY (owner_id) REFERENCES users(id) ON DELETE RESTRICT;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS ix_customers_tenant_owner
    ON customers (tenant_id, owner_id) WHERE is_deleted = FALSE;

CREATE INDEX IF NOT EXISTS ix_customers_tenant_team
    ON customers (tenant_id, team_id) WHERE is_deleted = FALSE;

-- ----------------------------------------------------------------------------
-- 5. Ghi vết lịch sử migration vào sys_schema_history (K14)
-- ----------------------------------------------------------------------------
INSERT INTO sys_schema_history (version, description)
VALUES ('V1.0.2', 'Add teams, users, CRM fields to customers')
ON CONFLICT (version) DO NOTHING;
```

Thực thi DDL bằng lệnh `psql` chuẩn UTF-8 ($[K25]$):

```powershell
psql -h localhost -U crm_user -d crm_db -P pager=off `
     -f docs/specs/ddl/V1.0.2__Add_CRM_Entities.sql
```

### 1.3 [EXEC] Cập nhật và tạo mới Entities tầng Domain (K16, K74)

**File 1: Cập nhật Entity `Customer`**

**Đường dẫn:** `src/Crm.Domain/Entities/Customer.cs`

```csharp
using System;

namespace Crm.Domain.Entities;

public class Customer
{
    // --- Các cột nền tảng Day 4 ---
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxCode { get; set; }
    public string? Address { get; set; }
    public string? Industry { get; set; }
    public int HealthStatus { get; set; }
    public decimal Revenue90d { get; set; }
    public DateTime CreatedAt { get; set; }

    // --- Các cột quản trị CRM Day 6 (K20, K21, K39) ---
    public Guid TenantId { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid? TeamId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public uint RowVersion { get; set; } // Map với PostgreSQL xmin Concurrency Token (K19)

    // ============================================
    // [K16] Navigation 2 chiều tường minh
    // ============================================
    public Team? Team { get; set; }
    public User? Owner { get; set; }
}
```

**File 2: Tạo mới Entity `Team`**

**Đường dẫn:** `src/Crm.Domain/Entities/Team.cs`

```csharp
using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentTeamId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    // [K16] Navigation tường minh — Bắt buộc khai báo đầy đủ collection đối ứng
    public Team? ParentTeam { get; set; }
    public ICollection<Team> ChildTeams { get; set; } = new List<Team>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
```

**File 3: Tạo mới Entity `User`**

**Đường dẫn:** `src/Crm.Domain/Entities/User.cs`

```csharp
using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? TeamId { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = "SALES";
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    // [K16] Navigation tường minh
    public Team? Team { get; set; }
    public ICollection<Customer> OwnedCustomers { get; set; } = new List<Customer>();
}
```

### 1.4 [VERIFY] Xác thực Phase 1 (K61, K63)

Chạy lệnh SQL đối soát 4 ràng buộc khóa ngoại tường minh trong CSDL:

```powershell
psql -h localhost -U crm_user -d crm_db -P pager=off -c "
SELECT conname, conrelid::regclass AS table_name
FROM pg_constraint
WHERE conname IN (
    'fk_customers_team_id', 'fk_customers_owner_id',
    'fk_users_team_id', 'fk_teams_parent_team_id'
)
ORDER BY conname;"
```

Kỳ vọng kết quả trả về đúng 4 dòng:

```text
> [PENDING REAL EXECUTION: Chờ kỹ sư chạy lệnh và paste output xác thực vào đây]
```

Biên dịch tầng Domain:

```powershell
dotnet build src/Crm.Domain/Crm.Domain.csproj -c Release
```

*Kỳ vọng:* `Build succeeded. 0 Warning(s). 0 Error(s).`

### 1.5 [ROLLBACK] Kịch bản hoàn tác Phase 1 (K54)

Nếu Phase 1 thất bại, thực thi lệnh sau để trả hệ thống về nguyên trạng:

```powershell
psql -h localhost -U crm_user -d crm_db -P pager=off -c "
ALTER TABLE customers DROP CONSTRAINT IF EXISTS fk_customers_team_id;
ALTER TABLE customers DROP CONSTRAINT IF EXISTS fk_customers_owner_id;
ALTER TABLE customers
  DROP COLUMN IF EXISTS tenant_id,
  DROP COLUMN IF EXISTS owner_id,
  DROP COLUMN IF EXISTS team_id,
  DROP COLUMN IF EXISTS is_deleted,
  DROP COLUMN IF EXISTS deleted_at,
  DROP COLUMN IF EXISTS deleted_by,
  DROP COLUMN IF EXISTS updated_at,
  DROP COLUMN IF EXISTS updated_by;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS teams;
DELETE FROM sys_schema_history WHERE version = 'V1.0.2';"

git restore src/Crm.Domain/Entities/Customer.cs
Remove-Item -Force src/Crm.Domain/Entities/Team.cs, src/Crm.Domain/Entities/User.cs
```

### 1.6 [LEARN] Cơ chế Shadow Property và Rủi ro Hệ thống (K16, K17)

EF Core là một ORM thông minh nhưng sự "ngầm định" của nó đôi khi dẫn tới những cạm bẫy nguy hiểm. Khi một entity cha (ví dụ `Team`) chứa `ICollection<Customer>`, nhưng entity con `Customer` không được chỉ định rõ ràng thuộc tính khóa ngoại nối về, EF Core sẽ ngầm tạo ra một cột ảo (shadow property như `TeamId1`).

Hậu quả thực tế là câu lệnh SQL sinh ra chứa các cột nối không hề có trong DDL thực tế, dữ liệu nạp lên bị `NULL` và lỗi này thường chỉ bộc phát khi đã triển khai lên Production. Quy tắc vàng ở đây rất đơn giản: **Luôn chỉ định cả hai đầu quan hệ bằng Fluent API kèm tên ràng buộc tường minh qua `.HasConstraintName()`**.

---

## PHASE 2 — DI CHUYỂN INTERFACES VÀ CẤU HÌNH DBCONTEXT (K12, K16, K17, K19, K39)

**Human Time Budget:** 20 phút

### 2.1 [SETUP] Giải quyết tận gốc Phụ thuộc Vòng (K03, K12, K71)

Trong kiến trúc Clean Architecture, nguyên tắc Dependency Inversion quy định rằng tầng lõi nghiệp vụ không được phụ thuộc vào tầng bên ngoài. Nếu ta đặt `ITenantProvider` ở tầng Business nhưng DbContext ở tầng Data lại cần nó để lọc dữ liệu, ta sẽ rơi vào cái bẫy phụ thuộc vòng (`Crm.Data` $\leftrightarrow$ `Crm.Business`). 

Bằng cách đưa các interface nhận diện ngữ cảnh dùng chung (`ITenantProvider`, `ICurrentUser`) về tầng `Crm.Domain`, ta tạo ra một điểm tựa kiến trúc vững chắc: Cả Data và Business đều phụ thuộc vào Domain, loại bỏ hoàn toàn nguy cơ lỗi biên dịch tuần hoàn `CS0246` hay `MSB4006`.

### 2.2 [EXEC] Tạo Interfaces tại tầng Domain (K12)

**File 1:** `src/Crm.Domain/Common/Interfaces/ITenantProvider.cs`

```csharp
using System;

namespace Crm.Domain.Common.Interfaces;

public interface ITenantProvider
{
    Guid TenantId { get; }
}
```

**File 2:** `src/Crm.Domain/Common/Interfaces/ICurrentUser.cs`

```csharp
using System;

namespace Crm.Domain.Common.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid? TeamId { get; }
    string Role { get; } // "SALES" | "MANAGER" | "ADMIN"
}
```

### 2.3 [EXEC] Cập nhật `AppDbContext` — Cấu hình tường minh (K16, K17, K19, K20, K39)

**Đường dẫn file:** `src/Crm.Data/AppDbContext.cs`

```csharp
using System;
using Crm.Domain.Common.Interfaces;
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Data;

public class AppDbContext : DbContext
{
    private readonly Guid _currentTenantId;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenant)
        : base(options)
    {
        _currentTenantId = tenant.TenantId;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ============================================
        // [K16, K17] CẤU HÌNH QUAN HỆ TƯỜNG MINH — 0 SHADOW PROPERTY
        // ============================================

        // Customer (N) -> Team (1)
        mb.Entity<Customer>()
            .HasOne(c => c.Team)
            .WithMany(t => t.Customers)
            .HasForeignKey(c => c.TeamId)
            .HasConstraintName("fk_customers_team_id")
            .OnDelete(DeleteBehavior.Restrict);

        // Customer (N) -> User (1, Owner)
        mb.Entity<Customer>()
            .HasOne(c => c.Owner)
            .WithMany(u => u.OwnedCustomers)
            .HasForeignKey(c => c.OwnerId)
            .HasConstraintName("fk_customers_owner_id")
            .OnDelete(DeleteBehavior.Restrict);

        // Team (Self-referencing: Con -> Cha)
        mb.Entity<Team>()
            .HasOne(t => t.ParentTeam)
            .WithMany(t => t.ChildTeams)
            .HasForeignKey(t => t.ParentTeamId)
            .HasConstraintName("fk_teams_parent_team_id")
            .OnDelete(DeleteBehavior.Restrict);

        // User (N) -> Team (1)
        mb.Entity<User>()
            .HasOne(u => u.Team)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TeamId)
            .HasConstraintName("fk_users_team_id")
            .OnDelete(DeleteBehavior.Restrict);

        // ============================================
        // [K39 - Lớp 1] Global Query Filter (Tenant Isolation + Soft Delete)
        // ============================================
        mb.Entity<Customer>()
            .HasQueryFilter(c => c.TenantId == _currentTenantId && !c.IsDeleted);

        mb.Entity<Team>()
            .HasQueryFilter(t => t.TenantId == _currentTenantId && !t.IsDeleted);

        mb.Entity<User>()
            .HasQueryFilter(u => u.TenantId == _currentTenantId && !u.IsDeleted);

        // ============================================
        // [K19] Optimistic Concurrency Token (PostgreSQL xmin)
        // ============================================
        mb.Entity<Customer>()
            .Property(c => c.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // ============================================
        // [K20] Đảm bảo DateTime luôn lưu dạng UTC TIMESTAMPTZ
        // ============================================
        foreach (var entityType in mb.Model.GetEntityTypes())
        {
            foreach (var prop in entityType.GetProperties())
            {
                if (prop.ClrType == typeof(DateTime) || prop.ClrType == typeof(DateTime?))
                {
                    prop.SetColumnType("TIMESTAMPTZ");
                }
            }
        }

        base.OnModelCreating(mb);
    }
}
```

### 2.4 [EXEC] Repository Interface & Implementation (K12, K18)

**File 1:** `src/Crm.Data/Repositories/ICustomerRepository.cs`

```csharp
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Domain.Entities;

namespace Crm.Data.Repositories;

public interface ICustomerRepository
{
    IQueryable<Customer> Query();
    Task<Customer?> GetByIdAsync(long id, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
    void Update(Customer customer);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
```

**File 2:** `src/Crm.Data/Repositories/CustomerRepository.cs`

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

    public CustomerRepository(AppDbContext db) => _db = db;

    public IQueryable<Customer> Query() => _db.Customers.AsQueryable();

    public Task<Customer?> GetByIdAsync(long id, CancellationToken ct)
        => _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Customer customer, CancellationToken ct)
        => await _db.Customers.AddAsync(customer, ct);

    public void Update(Customer customer) => _db.Customers.Update(customer);

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
```

### 2.5 [VERIFY] Xác thực Phase 2 (K61, K63)

```powershell
dotnet build src/Crm.Data/Crm.Data.csproj -c Release
```

*Kỳ vọng:* `Build succeeded. 0 Warning(s). 0 Error(s).`

### 2.6 [ROLLBACK] Kịch bản hoàn tác Phase 2 (K54)

```powershell
git restore src/Crm.Data/AppDbContext.cs
Remove-Item -Force src/Crm.Domain/Common/Interfaces/ITenantProvider.cs, `
                   src/Crm.Domain/Common/Interfaces/ICurrentUser.cs, `
                   src/Crm.Data/Repositories/ICustomerRepository.cs, `
                   src/Crm.Data/Repositories/CustomerRepository.cs
```

### 2.7 [DEVIATION] Xử lý sự cố Phase 2 (K65)

> ⚠ **[LỆCH CHUẨN DỰ KIẾN]: Lỗi `The property 'Customer.RowVersion' could not be mapped`.**
>
> * **Nguyên nhân gốc:** Thiếu mapping kiểu `xid` của PostgreSQL trong EF Core hoặc Npgsql version không tương thích.
> * **Khắc phục nhanh:** Kiểm tra dòng `.HasColumnType("xid")` trong cấu hình `mb.Entity<Customer>()`. Cột `xmin` là một system column đặc thù của PostgreSQL với kiểu dữ liệu là `xid`. Hãy kiểm tra lại xem trong `OnModelCreating` bạn đã gắn đủ `.HasColumnType("xid")` chưa, và đảm bảo package `Npgsql.EntityFrameworkCore.PostgreSQL` trong project đạt phiên bản 8.0 trở lên.

---

## PHASE 3 — MEDIATR HANDLERS + IDOR LỚP 2 (K12, K18, K37, K39)

**Human Time Budget:** 25 phút

### 3.1 [SETUP] Cài đặt Package và Tham chiếu (K50)

```powershell
dotnet add src/Crm.Business/Crm.Business.csproj package MediatR --version 12.4.1
dotnet add src/Crm.Business/Crm.Business.csproj reference src/Crm.Domain/Crm.Domain.csproj
dotnet add src/Crm.Business/Crm.Business.csproj reference src/Crm.Data/Crm.Data.csproj
```

### 3.2 [EXEC] Khởi tạo DTOs (K15)

**Đường dẫn file:** `src/Crm.Business/Customers/Dtos/CustomerDtos.cs`

```csharp
using System;
using System.Collections.Generic;

namespace Crm.Business.Customers.Dtos;

public sealed class CustomerListDto
{
    public long Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Industry { get; init; }
    public string? Phone { get; init; }
    public int HealthStatus { get; init; }
    public decimal Revenue90d { get; init; }
    public Guid? OwnerId { get; init; }
}

public sealed class CustomerFilterDto
{
    public int? HealthStatus { get; init; }
    public Guid? OwnerId { get; init; }
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

### 3.3 [EXEC] Query: `GetCustomerListQuery` (K12, K18, K39)

**File 1:** `src/Crm.Business/Customers/Queries/GetCustomerListQuery.cs`

```csharp
using System;
using Crm.Business.Customers.Dtos;
using MediatR;

namespace Crm.Business.Customers.Queries;

public sealed record GetCustomerListQuery(
    CustomerFilterDto Filter,
    Guid CurrentUserId,
    Guid? CurrentTeamId,
    string CurrentRole) : IRequest<PagedResult<CustomerListDto>>;
```

**File 2:** `src/Crm.Business/Customers/Queries/GetCustomerListQueryHandler.cs`

```csharp
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Business.Customers.Dtos;
using Crm.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crm.Business.Customers.Queries;

public sealed class GetCustomerListQueryHandler
    : IRequestHandler<GetCustomerListQuery, PagedResult<CustomerListDto>>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerListQueryHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<PagedResult<CustomerListDto>> Handle(
        GetCustomerListQuery request, CancellationToken ct)
    {
        var f = request.Filter;

        // [K18] Read-only tối ưu hóa bộ nhớ
        var query = _repo.Query().AsNoTracking();

        // [K39 - Lớp 2] IDOR: Phân quyền theo vai trò nghiệp vụ (Deny by default)
        query = request.CurrentRole switch
        {
            "SALES"   => query.Where(c => c.OwnerId == request.CurrentUserId),
            "MANAGER" => query.Where(c => c.TeamId == request.CurrentTeamId),
            "ADMIN"   => query,
            _         => query.Where(c => false)
        };

        if (f.HealthStatus.HasValue)
        {
            query = query.Where(c => c.HealthStatus == f.HealthStatus.Value);
        }

        if (f.OwnerId.HasValue)
        {
            query = query.Where(c => c.OwnerId == f.OwnerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(f.Keyword))
        {
            var kw = f.Keyword.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(kw) ||
                c.Code.ToLower().Contains(kw) ||
                (c.Phone != null && c.Phone.Contains(kw)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Skip((f.PageIndex - 1) * f.PageSize)
            .Take(f.PageSize)
            .Select(c => new CustomerListDto
            {
                Id           = c.Id,
                Code         = c.Code,
                Name         = c.Name,
                Industry     = c.Industry,
                Phone        = c.Phone,
                HealthStatus = c.HealthStatus,
                Revenue90d   = c.Revenue90d,
                OwnerId      = c.OwnerId
            })
            .ToListAsync(ct);

        return new PagedResult<CustomerListDto>
        {
            Items      = items,
            TotalCount = total,
            PageIndex  = f.PageIndex,
            PageSize   = f.PageSize
        };
    }
}
```

### 3.4 [EXEC] Command: `CreateCustomerCommand` (K12, K20, K37, K39)

**File 1:** `src/Crm.Business/Customers/Commands/CreateCustomerCommand.cs`

```csharp
using MediatR;

namespace Crm.Business.Customers.Commands;

public sealed record CreateCustomerCommand(
    string Name, string? Phone, string? Email, string? TaxCode,
    string? Address, string? Industry, int HealthStatus) : IRequest<long>;
```

**File 2:** `src/Crm.Business/Customers/Commands/CreateCustomerCommandHandler.cs`

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Crm.Data.Repositories;
using Crm.Domain.Common.Interfaces;
using Crm.Domain.Entities;
using MediatR;

namespace Crm.Business.Customers.Commands;

public sealed class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, long>
{
    private readonly ICustomerRepository _repo;
    private readonly ICurrentUser _user;
    private readonly ITenantProvider _tenant;

    public CreateCustomerCommandHandler(
        ICustomerRepository repo, ICurrentUser user, ITenantProvider tenant)
        => (_repo, _user, _tenant) = (repo, user, tenant);

    public async Task<long> Handle(CreateCustomerCommand cmd, CancellationToken ct)
    {
        // [K37] Server-side validation
        if (string.IsNullOrWhiteSpace(cmd.Name))
        {
            throw new ArgumentException("Tên khách hàng không được để trống", nameof(cmd.Name));
        }

        if (!string.IsNullOrWhiteSpace(cmd.Phone) && cmd.Phone.Length < 10)
        {
            throw new ArgumentException("Số điện thoại không hợp lệ", nameof(cmd.Phone));
        }

        var customer = new Customer
        {
            Code         = "PENDING",
            Name         = cmd.Name.Trim(),
            Phone        = cmd.Phone,
            Email        = cmd.Email,
            TaxCode      = cmd.TaxCode,
            Address      = cmd.Address,
            Industry     = cmd.Industry,
            HealthStatus = cmd.HealthStatus,
            OwnerId      = _user.UserId,
            TeamId       = _user.TeamId,
            TenantId     = _tenant.TenantId,
            CreatedAt    = DateTime.UtcNow,
            IsDeleted    = false
        };

        await _repo.AddAsync(customer, ct);
        await _repo.SaveChangesAsync(ct);

        // Sinh mã khách hàng 2-phase format
        customer.Code = $"KH-{customer.Id:D4}";
        await _repo.SaveChangesAsync(ct);

        return customer.Id;
    }
}
```

### 3.5 [EXEC] Command: `UpdateCustomerCommand` (K12, K19, K37, K39)

**File 1:** `src/Crm.Business/Customers/Commands/UpdateCustomerCommand.cs`

```csharp
using MediatR;

namespace Crm.Business.Customers.Commands;

public sealed record UpdateCustomerCommand(
    long Id, string Name, string? Phone, string? Email,
    string? TaxCode, string? Address, string? Industry, int HealthStatus)
    : IRequest<Unit>;
```

**File 2:** `src/Crm.Business/Customers/Commands/UpdateCustomerCommandHandler.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Crm.Data.Repositories;
using Crm.Domain.Common.Interfaces;
using MediatR;

namespace Crm.Business.Customers.Commands;

public sealed class UpdateCustomerCommandHandler
    : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly ICustomerRepository _repo;
    private readonly ICurrentUser _user;

    public UpdateCustomerCommandHandler(ICustomerRepository repo, ICurrentUser user)
        => (_repo, _user) = (repo, user);

    public async Task<Unit> Handle(UpdateCustomerCommand cmd, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy khách hàng với Id={cmd.Id}");

        // [K39 - Lớp 2] IDOR: Sales chỉ được sửa khách hàng của chính mình
        if (_user.Role == "SALES" && customer.OwnerId != _user.UserId)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa khách hàng này.");
        }

        if (string.IsNullOrWhiteSpace(cmd.Name))
        {
            throw new ArgumentException("Tên khách hàng không được để trống", nameof(cmd.Name));
        }

        customer.Name         = cmd.Name.Trim();
        customer.Phone        = cmd.Phone;
        customer.Email        = cmd.Email;
        customer.TaxCode      = cmd.TaxCode;
        customer.Address      = cmd.Address;
        customer.Industry     = cmd.Industry;
        customer.HealthStatus = cmd.HealthStatus;
        customer.UpdatedAt    = DateTime.UtcNow;
        customer.UpdatedBy    = _user.UserId;

        _repo.Update(customer);
        await _repo.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
```

### 3.6 [VERIFY] Xác thực Phase 3 (K61, K63)

```powershell
dotnet build src/Crm.Business/Crm.Business.csproj -c Release
```

*Kỳ vọng:* `Build succeeded. 0 Warning(s). 0 Error(s).`

### 3.7 [ROLLBACK] Kịch bản hoàn tác Phase 3 (K54)

```powershell
Remove-Item -Recurse -Force src/Crm.Business/Customers
```

---

## PHASE 4 — DI REGISTRATION VÀ CẤU HÌNH WEB ADAPTERS (K12, K39)

**Human Time Budget:** 10 phút

### 4.1 [EXEC] Triển khai Web Adapters

**File 1: HttpTenantProvider**

**Đường dẫn:** `src/Crm.Web/Services/HttpTenantProvider.cs`

```csharp
using System;
using Crm.Domain.Common.Interfaces;

namespace Crm.Web.Services;

public sealed class HttpTenantProvider : ITenantProvider
{
    // Sprint 1 thiết lập tenant mặc định. Sprint 3+ đọc từ header hoặc JWT claim
    public Guid TenantId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
}
```

**File 2: HttpCurrentUser**

**Đường dẫn:** `src/Crm.Web/Services/HttpCurrentUser.cs`

```csharp
using System;
using System.Security.Claims;
using Crm.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace Crm.Web.Services;

public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly AuthenticationStateProvider _auth;

    public HttpCurrentUser(AuthenticationStateProvider auth) => _auth = auth;

    private ClaimsPrincipal User =>
        _auth.GetAuthenticationStateAsync().GetAwaiter().GetResult().User;

    public Guid UserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var g) ? g : Guid.Empty;

    public Guid? TeamId =>
        Guid.TryParse(User.FindFirstValue("team_id"), out var g) ? g : null;

    public string Role => User.FindFirstValue(ClaimTypes.Role) ?? "SALES";
}
```

### 4.2 [EXEC] Cập nhật DI Container trong `Program.cs` (K12)

**Đường dẫn:** `src/Crm.Web/Program.cs` (Thêm khối đăng ký trước `builder.Build()`)

```csharp
using Crm.Business.Customers.Commands;
using Crm.Data;
using Crm.Data.Repositories;
using Crm.Domain.Common.Interfaces;
using Crm.Web.Services;

// [K12] MediatR Handlers registration
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly));

// [K16, K39] Repositories & Security Context (Đăng ký đúng Scoped)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITenantProvider, HttpTenantProvider>();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
```

### 4.3 [VERIFY] Xác thực Phase 4 (K61, K63)

```powershell
dotnet build src/Crm.Web/Crm.Web.csproj -c Release
```

*Kỳ vọng:* `Build succeeded. 0 Warning(s). 0 Error(s).`

### 4.4 [ROLLBACK] Kịch bản hoàn tác Phase 4 (K54)

```powershell
Remove-Item -Force src/Crm.Web/Services/HttpTenantProvider.cs, src/Crm.Web/Services/HttpCurrentUser.cs
git restore src/Crm.Web/Program.cs
```

---

## PHASE 5 — TOÀN BỘ 19 UNIT TESTS ĐỘC LẬP (K45, K46, K63)

**Human Time Budget:** 25 phút

### 5.1 [SETUP] Cài đặt Package Test và Tham chiếu (K50)

```powershell
dotnet add tests/Crm.Tests/Crm.Tests.csproj package Moq --version 4.20.72
dotnet add tests/Crm.Tests/Crm.Tests.csproj package FluentAssertions --version 6.12.2
dotnet add tests/Crm.Tests/Crm.Tests.csproj package MockQueryable.Moq --version 7.0.3
dotnet add tests/Crm.Tests/Crm.Tests.csproj reference src/Crm.Domain/Crm.Domain.csproj
dotnet add tests/Crm.Tests/Crm.Tests.csproj reference src/Crm.Data/Crm.Data.csproj
dotnet add tests/Crm.Tests/Crm.Tests.csproj reference src/Crm.Business/Crm.Business.csproj
```

### 5.2 [EXEC] Test Suite 1: Query Handler (7 Tests)

**Đường dẫn file:** `tests/Crm.Tests/Customers/GetCustomerListQueryHandlerTests.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Crm.Business.Customers.Dtos;
using Crm.Business.Customers.Queries;
using Crm.Data.Repositories;
using Crm.Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Crm.Tests.Customers;

public class GetCustomerListQueryHandlerTests
{
    private static readonly Guid Sales1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Sales2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid TeamA  = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static GetCustomerListQueryHandler BuildSut()
    {
        var data = new List<Customer>
        {
            new() { Id=1, Code="KH-0001", Name="An Phát",   OwnerId=Sales1, TeamId=TeamA, HealthStatus=1, IsDeleted=false, CreatedAt=DateTime.UtcNow },
            new() { Id=2, Code="KH-0002", Name="Việt Nhật", OwnerId=Sales1, TeamId=TeamA, HealthStatus=1, IsDeleted=false, CreatedAt=DateTime.UtcNow },
            new() { Id=3, Code="KH-0003", Name="Tân Á",     OwnerId=Sales2, TeamId=TeamA, HealthStatus=2, IsDeleted=false, CreatedAt=DateTime.UtcNow },
        }.BuildMock();

        var repo = new Mock<ICustomerRepository>();
        repo.Setup(r => r.Query()).Returns(data);
        return new GetCustomerListQueryHandler(repo.Object);
    }

    [Fact]
    public async Task Sales_ChiThayKhachCuaMinh()
    {
        var sut = BuildSut();
        var result = await sut.Handle(
            new GetCustomerListQuery(new CustomerFilterDto(), Sales1, TeamA, "SALES"),
            CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(c => c.OwnerId == Sales1);
    }

    [Fact]
    public async Task Manager_ThayToanTeam()
    {
        var sut = BuildSut();
        var result = await sut.Handle(
            new GetCustomerListQuery(new CustomerFilterDto(), Guid.NewGuid(), TeamA, "MANAGER"),
            CancellationToken.None);

        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Admin_ThayTatCa()
    {
        var sut = BuildSut();
        var result = await sut.Handle(
            new GetCustomerListQuery(new CustomerFilterDto(), Guid.NewGuid(), TeamA, "ADMIN"),
            CancellationToken.None);

        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Role_KhongHopLe_DenyByDefault()
    {
        var sut = BuildSut();
        var result = await sut.Handle(
            new GetCustomerListQuery(new CustomerFilterDto(), Guid.NewGuid(), TeamA, "UNKNOWN_ROLE"),
            CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Filter_TheoHealthStatus()
    {
        var sut = BuildSut();
        var result = await sut.Handle(
            new GetCustomerListQuery(new CustomerFilterDto { HealthStatus = 2 }, Guid.NewGuid(), TeamA, "ADMIN"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Tân Á");
    }

    [Fact]
    public async Task Search_TheoKeyword()
    {
        var sut = BuildSut();
        var result = await sut.Handle(
            new GetCustomerListQuery(new CustomerFilterDto { Keyword = "phát" }, Guid.NewGuid(), TeamA, "ADMIN"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Code.Should().Be("KH-0001");
    }

    [Fact]
    public async Task Pagination_HoatDongChinhXac()
    {
        var sut = BuildSut();
        var result = await sut.Handle(
            new GetCustomerListQuery(new CustomerFilterDto { PageSize = 2, PageIndex = 1 }, Guid.NewGuid(), TeamA, "ADMIN"),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
    }
}
```

### 5.3 [EXEC] Test Suite 2: Create Handler (3 Tests)

**Đường dẫn file:** `tests/Crm.Tests/Customers/CreateCustomerCommandHandlerTests.cs`

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Crm.Business.Customers.Commands;
using Crm.Data.Repositories;
using Crm.Domain.Common.Interfaces;
using Crm.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Crm.Tests.Customers;

public class CreateCustomerCommandHandlerTests
{
    private static (CreateCustomerCommandHandler sut, Mock<ICustomerRepository> repo) Build(long newId = 42)
    {
        var repo = new Mock<ICustomerRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>((c, _) => c.Id = newId)
            .Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var user = new Mock<ICurrentUser>();
        user.SetupGet(u => u.UserId).Returns(Guid.NewGuid());
        user.SetupGet(u => u.TeamId).Returns(Guid.NewGuid());

        var tenant = new Mock<ITenantProvider>();
        tenant.SetupGet(t => t.TenantId).Returns(Guid.NewGuid());

        return (new CreateCustomerCommandHandler(repo.Object, user.Object, tenant.Object), repo);
    }

    [Fact]
    public async Task TaoKH_SinhMaDungFormat()
    {
        var (sut, repo) = Build(42);

        var id = await sut.Handle(
            new CreateCustomerCommand("Công ty ABC", "0901234567", null, null, null, null, 1),
            CancellationToken.None);

        id.Should().Be(42);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task TaoKH_NemLoi_KhiPhoneKhongHopLe()
    {
        var (sut, _) = Build();

        var act = async () => await sut.Handle(
            new CreateCustomerCommand("Công ty DEF", "123", null, null, null, null, 1),
            CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*Số điện thoại không hợp lệ*");
    }

    [Fact]
    public async Task TaoKH_NemLoi_KhiNameTrong()
    {
        var (sut, _) = Build();

        var act = async () => await sut.Handle(
            new CreateCustomerCommand("", "0901234567", null, null, null, null, 1),
            CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*Tên khách hàng*");
    }
}
```

### 5.4 [EXEC] Test Suite 3: Update Handler & IDOR Lớp 2 (4 Tests)

**Đường dẫn file:** `tests/Crm.Tests/Customers/UpdateCustomerCommandHandlerTests.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Crm.Business.Customers.Commands;
using Crm.Data.Repositories;
using Crm.Domain.Common.Interfaces;
using Crm.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Crm.Tests.Customers;

public class UpdateCustomerCommandHandlerTests
{
    private static readonly Guid SalesUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherUserId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    [Fact]
    public async Task SuaKH_ThanhCong_KhiDuQuyen()
    {
        var existing = new Customer { Id = 10, Name = "Cũ", OwnerId = SalesUserId };
        var repo = new Mock<ICustomerRepository>();
        repo.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var user = new Mock<ICurrentUser>();
        user.SetupGet(u => u.UserId).Returns(SalesUserId);
        user.SetupGet(u => u.Role).Returns("SALES");

        var sut = new UpdateCustomerCommandHandler(repo.Object, user.Object);
        await sut.Handle(new UpdateCustomerCommand(10, "Mới", null, null, null, null, null, 2), CancellationToken.None);

        existing.Name.Should().Be("Mới");
        existing.UpdatedAt.Should().NotBeNull();
        existing.UpdatedBy.Should().Be(SalesUserId);
        repo.Verify(r => r.Update(existing), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SuaKH_NemUnauthorized_KhiSalesSuaKhachNguoiKhac()
    {
        var existing = new Customer { Id = 10, Name = "Cũ", OwnerId = OtherUserId };
        var repo = new Mock<ICustomerRepository>();
        repo.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var user = new Mock<ICurrentUser>();
        user.SetupGet(u => u.UserId).Returns(SalesUserId);
        user.SetupGet(u => u.Role).Returns("SALES");

        var sut = new UpdateCustomerCommandHandler(repo.Object, user.Object);
        var act = async () => await sut.Handle(
            new UpdateCustomerCommand(10, "Tấn công IDOR", null, null, null, null, null, 1), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
                 .WithMessage("*không có quyền chỉnh sửa*");
    }

    [Fact]
    public async Task SuaKH_NemKeyNotFound_KhiKhachKhongTonTai()
    {
        var repo = new Mock<ICustomerRepository>();
        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);

        var user = new Mock<ICurrentUser>();
        var sut = new UpdateCustomerCommandHandler(repo.Object, user.Object);

        var act = async () => await sut.Handle(
            new UpdateCustomerCommand(99, "Tên", null, null, null, null, null, 1), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task SuaKH_NemArgumentException_KhiNameRong()
    {
        var existing = new Customer { Id = 10, Name = "Cũ", OwnerId = SalesUserId };
        var repo = new Mock<ICustomerRepository>();
        repo.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var user = new Mock<ICurrentUser>();
        user.SetupGet(u => u.UserId).Returns(SalesUserId);
        user.SetupGet(u => u.Role).Returns("SALES");

        var sut = new UpdateCustomerCommandHandler(repo.Object, user.Object);
        var act = async () => await sut.Handle(
            new UpdateCustomerCommand(10, "", null, null, null, null, null, 1), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*Tên khách hàng*");
    }
}
```

### 5.5 [EXEC] Test Suite 4: Model Configuration — 0 Shadow Property (5 Tests)

**Đường dẫn file:** `tests/Crm.Tests/Data/ModelConfigurationTests.cs`

```csharp
using System;
using System.Linq;
using Crm.Data;
using Crm.Domain.Common.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Crm.Tests.Data;

public class ModelConfigurationTests
{
    [Fact]
    public void Model_KhongCoShadowProperty()
    {
        using var db = CreateDbContext();
        var shadowProps = db.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties().Where(p => p.IsShadowProperty()))
            .Select(p => $"{p.DeclaringType.ClrType.Name}.{p.Name}")
            .ToList();

        shadowProps.Should().BeEmpty("EF Core không được tự sinh shadow property — [K17]");
    }

    [Fact]
    public void Customer_TeamQuanHe_TuongMinh()
    {
        using var db = CreateDbContext();
        var fk = db.Model.FindEntityType(typeof(Crm.Domain.Entities.Customer))!
            .GetForeignKeys()
            .FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Crm.Domain.Entities.Team));

        fk.Should().NotBeNull();
        fk!.Properties.Should().ContainSingle(p => p.Name == "TeamId");
        fk.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
        fk.GetConstraintName().Should().Be("fk_customers_team_id");
    }

    [Fact]
    public void Customer_OwnerQuanHe_TuongMinh()
    {
        using var db = CreateDbContext();
        var fk = db.Model.FindEntityType(typeof(Crm.Domain.Entities.Customer))!
            .GetForeignKeys()
            .FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Crm.Domain.Entities.User));

        fk.Should().NotBeNull();
        fk!.Properties.Should().ContainSingle(p => p.Name == "OwnerId");
        fk.GetConstraintName().Should().Be("fk_customers_owner_id");
    }

    [Fact]
    public void Team_TuThamChieu_TuongMinh()
    {
        using var db = CreateDbContext();
        var fk = db.Model.FindEntityType(typeof(Crm.Domain.Entities.Team))!
            .GetForeignKeys()
            .FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Crm.Domain.Entities.Team));

        fk.Should().NotBeNull();
        fk!.Properties.Should().ContainSingle(p => p.Name == "ParentTeamId");
        fk.GetConstraintName().Should().Be("fk_teams_parent_team_id");
    }

    [Fact]
    public void User_TeamQuanHe_TuongMinh()
    {
        using var db = CreateDbContext();
        var fk = db.Model.FindEntityType(typeof(Crm.Domain.Entities.User))!
            .GetForeignKeys()
            .FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Crm.Domain.Entities.Team));

        fk.Should().NotBeNull();
        fk!.Properties.Should().ContainSingle(p => p.Name == "TeamId");
        fk.GetConstraintName().Should().Be("fk_users_team_id");
    }

    private static AppDbContext CreateDbContext()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=test_metadata;Username=test;Password=test")
            .Options;
        return new AppDbContext(opts, new FakeTenantProvider());
    }

    private sealed class FakeTenantProvider : ITenantProvider
    {
        public Guid TenantId => Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
```

### 5.6 [VERIFY] Xác thực Phase 5 (K61, K63)

```powershell
dotnet test tests/Crm.Tests/Crm.Tests.csproj -c Release --logger "console;verbosity=normal"
```

Chốt chặn kết quả thực nghiệm:

```text
> [PENDING REAL EXECUTION: Chờ kỹ sư chạy lệnh và paste output xác thực vào đây]
```

*Kỳ vọng đích:* `Passed! - Failed: 0, Passed: 19, Skipped: 0, Total: 19`

### 5.7 [ROLLBACK] Kịch bản hoàn tác Phase 5 (K54)

```powershell
Remove-Item -Recurse -Force tests/Crm.Tests/Customers, tests/Crm.Tests/Data
```

---

## PHASE 6 — PRE-COMMIT CHAIN + ĐỒNG BỘ TÀI LIỆU + GIT COMMIT (K08, K15, K48, K49)

**Human Time Budget:** 15 phút

### 6.1 [EXEC] Chạy chuỗi Pre-Commit Chain 6 bước tự động (K49)

```powershell
Set-Location $slnRoot

# Bước 1 — Kiểm tra Code Style & Format chuẩn mực (K49.1)
dotnet format --verify-no-changes
if ($LASTEXITCODE -ne 0) { throw "[PRE-COMMIT BƯỚC 1 FAIL] Code chưa format đúng chuẩn." }

# Bước 2 — Biên dịch Release 0 Warning 0 Error (K49.2)
dotnet build -c Release
if ($LASTEXITCODE -ne 0) { throw "[PRE-COMMIT BƯỚC 2 FAIL] Biên dịch phát sinh lỗi." }

# Bước 3 — Thực thi toàn bộ 19 Unit Tests (K49.3)
dotnet test -c Release --no-build
if ($LASTEXITCODE -ne 0) { throw "[PRE-COMMIT BƯỚC 3 FAIL] Có ca kiểm thử không vượt qua." }

# Bước 4 — Quét Secret & Khóa cứng (K49.4)
$secretPatterns = @(
    'password\s*=\s*"[^"]+"',
    'apikey\s*=\s*"[^"]+"',
    'secret\s*=\s*"[^"]+"',
    'Bearer\s+[A-Za-z0-9\-\._~\+\/]+'
)
$violations = @()
Get-ChildItem -Recurse -Include *.cs,*.json,*.sql |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    ForEach-Object {
        foreach ($p in $secretPatterns) {
            if (Select-String -Path $_.FullName -Pattern $p -Quiet) {
                $violations += $_.FullName
                break
            }
        }
    }
if ($violations.Count -gt 0) { throw "[PRE-COMMIT BƯỚC 4 FAIL] Phát hiện secret tại $($violations -join ', ')" }

# Bước 5 — Audit giao diện Razor tĩnh (K35, K49.5)
$uiViolations = Get-ChildItem -Recurse -Include *.razor |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    Select-String -Pattern 'style\s*=\s*"[^@][^"]*"' -List
if ($uiViolations) { throw "[PRE-COMMIT BƯỚC 5 FAIL] Vi phạm inline CSS tĩnh tại $($uiViolations.Path -join ', ')" }

# Bước 6 — Kiểm tra trôi tài liệu Doc Drift Scan (K49.6)
$hasDdl = Get-ChildItem -Recurse -Include V*.sql
$hasDto = Get-ChildItem -Recurse -Include *.cs |
    Select-String -Pattern "record .*Command|record .*Query" -Quiet
if ($hasDdl -or $hasDto) {
    if (-not (Test-Path "docs/specs/api_contracts.md")) { throw "[PRE-COMMIT BƯỚC 6 FAIL] Thiếu api_contracts.md" }
    if (-not (Test-Path "docs/specs/data_dictionary.md")) { throw "[PRE-COMMIT BƯỚC 6 FAIL] Thiếu data_dictionary.md" }
}

Write-Host ">> PRE-COMMIT CHAIN: 6/6 BƯỚC PASS HOÀN TOÀN" -ForegroundColor Green
```

### 6.2 [EXEC] Đồng bộ Từ điển dữ liệu (K15)

**Đường dẫn file:** `docs/specs/data_dictionary.md` (Ghi nhận thêm bảng và cột mới)

```markdown
## customers — Cập nhật V1.0.2 (2026-10-05)

| Cột | Kiểu dữ liệu | Nullable | Ghi chú kỹ thuật |
|:---|:---|:---|:---|
| tenant_id | UUID | NO | [K39] Phân lập dữ liệu tổ chức |
| owner_id | UUID | YES | [K39] Khóa ngoại `fk_customers_owner_id` $\rightarrow$ users(id) |
| team_id | UUID | YES | [K39] Khóa ngoại `fk_customers_team_id` $\rightarrow$ teams(id) |
| is_deleted | BOOLEAN | NO | [K21] Cờ đánh dấu xóa mềm |
| deleted_at | TIMESTAMPTZ | YES | Thời điểm xóa mềm |
| deleted_by | UUID | YES | Người thực hiện xóa |
| updated_at | TIMESTAMPTZ | YES | [K20] Thời điểm sửa đổi gần nhất (UTC) |
| updated_by | UUID | YES | Người thực hiện sửa đổi |
| xmin | xid | NO | [K19] System column PostgreSQL làm Optimistic Concurrency Token |

## teams — Mới V1.0.2

| Cột | Kiểu dữ liệu | Nullable | Ghi chú kỹ thuật |
|:---|:---|:---|:---|
| id | UUID | NO | Khóa chính (gen_random_uuid()) |
| tenant_id | UUID | NO | Phân lập tenant |
| name | VARCHAR(200) | NO | Tên phòng ban / đội ngũ |
| parent_team_id | UUID | YES | Tự tham chiếu `fk_teams_parent_team_id` $\rightarrow$ teams(id) |
| is_deleted | BOOLEAN | NO | Xóa mềm |
| created_at | TIMESTAMPTZ | NO | Thời điểm tạo (UTC) |

## users — Mới V1.0.2

| Cột | Kiểu dữ liệu | Nullable | Ghi chú kỹ thuật |
|:---|:---|:---|:---|
| id | UUID | NO | Khóa chính (gen_random_uuid()) |
| tenant_id | UUID | NO | Phân lập tenant |
| team_id | UUID | YES | Khóa ngoại `fk_users_team_id` $\rightarrow$ teams(id) |
| email | VARCHAR(200) | NO | Duy nhất (uq_users_email) |
| role | VARCHAR(20) | NO | Vai trò: SALES, MANAGER, ADMIN |
| is_deleted | BOOLEAN | NO | Xóa mềm |
| created_at | TIMESTAMPTZ | NO | Thời điểm tạo (UTC) |
```

### 6.3 [EXEC] Đồng bộ Hợp đồng API (K15)

**Đường dẫn file:** `docs/specs/api_contracts.md` (Append)

```markdown
## Khối nghiệp vụ Customer CQRS — Day 6

### Queries
- `GetCustomerListQuery(CustomerFilterDto Filter, Guid CurrentUserId, Guid? CurrentTeamId, string CurrentRole)`
  - **Output:** `PagedResult<CustomerListDto>`
  - **Quy tắc bảo mật IDOR Lớp 2:** SALES (chỉ thấy khách của mình), MANAGER (toàn team), ADMIN (toàn tenant), Role khác $\rightarrow$ Deny by default (rỗng).

### Commands
- `CreateCustomerCommand(string Name, string? Phone, string? Email, string? TaxCode, string? Address, string? Industry, int HealthStatus)`
  - **Output:** `long` (Customer Id)
  - **Mã khách hàng:** Sinh tự động 2-phase format `KH-{Id:D4}`.
- `UpdateCustomerCommand(long Id, string Name, string? Phone, string? Email, string? TaxCode, string? Address, string? Industry, int HealthStatus)`
  - **Output:** `Unit`
  - **Kiểm soát IDOR Lớp 2:** Ném `UnauthorizedAccessException` nếu SALES sửa khách không do mình quản lý.

### Ngoại lệ chuẩn hóa
- `ArgumentException`: Vi phạm quy tắc xác thực dữ liệu đầu vào.
- `UnauthorizedAccessException`: Vi phạm IDOR Lớp 2.
- `KeyNotFoundException`: Không tìm thấy bản ghi khách hàng.
- `DbUpdateConcurrencyException`: Xung đột ghi đồng thời qua Concurrency Token `xmin`.
```

### 6.4 [EXEC] Ghi nhận Nợ kỹ thuật — Tech Debt (K07)

**Đường dẫn file:** `docs/sprints/sprint_1/tech_debt_report.md`

| \# | Vị trí Mã nguồn | Lý do Tạm thời | Giải pháp Lâu dài | Thời hạn Xử lý |
| :---: | :--- | :--- | :--- | :---: |
| 1 | `HttpTenantProvider.TenantId` | Hardcode GUID cho Sprint 1 đơn tenant | Đọc động từ Subdomain và JWT Claim | Sprint 3 |
| 2 | `CreateCustomerCommandHandler` | Lưu 2 lần (2-phase save) để lấy Id sinh Code | Chuyển sang PostgreSQL Sequence `NEXTVAL` trước khi insert | Sprint 2 |
| 3 | `ModelConfigurationTests` | Dùng chuỗi kết nối Npgsql giả lập để trích xuất metadata | Tích hợp Testcontainers với PostgreSQL thật | Sprint 2 |

### 6.5 [EXEC] Sổ tay Kiến thức — Concepts Mastered (K69)

**Đường dẫn file:** `docs/notes/concepts_mastered.md` (Append)

```markdown
## Day 6 — 2026-10-05
1. **CQRS Tách biệt Tuyệt đối (K12):** Query chỉ đọc dùng `.AsNoTracking()` tối ưu tài nguyên; Command xử lý giao dịch và concurrency.
2. **Phòng thủ IDOR 2 Lớp (K39):** Lớp 1 (EF Core Global Query Filter) ngăn rò rỉ dữ liệu chéo Tenant; Lớp 2 (Handler Role Check) bảo vệ quyền sở hữu dữ liệu cấp nhân sự.
3. **Điều hướng Tường minh (K16, K17):** Luôn khai báo đầy đủ collection đối ứng và `.HasForeignKey()` kèm `.HasConstraintName()` để xóa sổ $100\%$ shadow property ngầm.
4. **Optimistic Concurrency với PostgreSQL xmin (K19):** Sử dụng cột hệ thống `xmin` làm concurrency token mà không cần tạo thêm cột trên bảng vật lý.
5. **Clean Architecture Dependency Flow:** Đặt interface dùng chung (`ITenantProvider`, `ICurrentUser`) tại Domain để ngăn chặn triệt để phụ thuộc vòng giữa Data và Business.
```

### 6.6 [EXEC] Nhật ký Xoay vòng 3 Ngày — Daily Log (K70)

**Đường dẫn file:** `docs/notes/daily.md` (Duy trì cửa sổ tối đa 3 ngày gần nhất)

```markdown
## Day 6 — 2026-10-05 / Sprint 1
### Đã hoàn thành (Done)
- [x] DDL V1.0.2: Khởi tạo `teams`, `users`, bổ sung 8 cột CRM cho `customers`.
- [x] Entities Domain: `Customer`, `Team`, `User` với navigation 2 chiều tường minh.
- [x] Di chuyển `ITenantProvider`, `ICurrentUser` vào `Crm.Domain` triệt tiêu phụ thuộc vòng.
- [x] Cấu hình `OnModelCreating`: 4 Foreign Keys tường minh, kiểm định 0 shadow property.
- [x] Global Query Filter (Tenant Isolation + Soft Delete) và Concurrency Token `xmin`.
- [x] Triển khai CQRS Handlers: `GetCustomerListQuery`, `CreateCustomerCommand`, `UpdateCustomerCommand`.
- [x] Kiểm soát IDOR 2 lớp: Sales chỉ truy cập và sửa đổi khách hàng của mình.
- [x] 19 unit tests pass 100% (7 Query + 3 Create + 4 Update + 5 Model Metadata).
- [x] Pre-commit chain 6/6 bước xanh tuyệt đối.
- [x] Đồng bộ đầy đủ Data Dictionary, API Contracts, Tech Debt, Concepts Mastered.

### Chỉ số Nghiệm thu
- Mã nguồn & Schema CSDL: 19 files
- Bộ kiểm thử: 4 files
- Tài liệu đặc tả & vận hành: 5 files
- Tổng file thay đổi: 28 files
- Unit Test: 19/19 Pass (0 Fail, 0 Skip)
- Build: 0 Warning, 0 Error
- Shadow Property: 0

### Kế hoạch Tiếp theo (Day 7)
- Xây dựng giao diện Blazor `/customers` (K26–K35).
- Thiết kế 4 trạng thái giao diện: Loading, Empty, Error, Success (K27).
- Chuẩn hóa Responsive Mobile-first breakpoint 375px (K28).
- Triển khai Route Guard bảo vệ quyền truy cập trên UI.
```

### 6.7 [EXEC] Commit mã nguồn theo chuẩn Conventional Commits (K08)

```powershell
git checkout -b feature/day6-customer-cqrs
git add .
git commit -m "feat(customer): triển khai CQRS handlers, IDOR 2 lớp, model tường minh và 19 tests

- DDL V1.0.2: thêm bảng teams, users và 8 cột CRM cho customers
- Chuyển interface ITenantProvider vào Domain loại bỏ phụ thuộc vòng
- Cấu hình OnModelCreating tường minh 4 khóa ngoại, triệt tiêu shadow property
- Kích hoạt Global Query Filter và Optimistic Concurrency qua cột hệ thống xmin
- Xây dựng Handlers: GetCustomerList, CreateCustomer, UpdateCustomer
- Thiết lập IDOR 2 lớp: Global Query Filter và Role-based ownership check
- Phủ 19 Unit Tests (14 Handlers + 5 Model Metadata)
- Pre-commit chain 6/6 bước kiểm tra thành công
- Đồng bộ tài liệu data_dictionary, api_contracts và ghi nhận tech debt

DoD: Build Release 0W0E, 19/19 tests pass, 0 shadow property, docs synced"
```

---

## 7. MA TRẬN TRA CỨU & XỬ LÝ SỰ CỐ (TROUBLESHOOTING MATRIX — K60, K64)

| \# | Triệu chứng (Symptom) | Nguyên nhân gốc (Root Cause) | Giải pháp xử lý (Fix Action) | Xác nhận (Verification) |
| :---: | :--- | :--- | :--- | :--- |
| 1 | `MSB4006: Circular dependency` | `Crm.Data` và `Crm.Business` phụ thuộc chéo qua interface | Chuyển `ITenantProvider` vào `Crm.Domain.Common.Interfaces` | `dotnet build` hoàn tất 0 error |
| 2 | Sales xem được khách hàng của người khác | Thiếu logic lọc quyền trong `GetCustomerListQueryHandler` | Thêm mệnh đề `switch-case` role với điều kiện `c.OwnerId == CurrentUserId` | Test `Sales_ChiThayKhachCuaMinh` PASS |
| 3 | `The property 'RowVersion' could not be mapped` | Chưa chỉ định kiểu dữ liệu tương thích với PostgreSQL `xid` | Bổ sung `.HasColumnType("xid")` trong `AppDbContext` | Biên dịch thành công, test pass |
| 4 | Xuất hiện Shadow Property `TeamId1` | Collection đối ứng không được chỉ rõ trong cấu hình Fluent API | Khai báo đầy đủ `.WithMany(t => t.Customers).HasForeignKey(c => c.TeamId)` | Test `Model_KhongCoShadowProperty` PASS |
| 5 | `Cannot resolve scoped service from root` | Đăng ký dịch vụ Scoped vào container dưới dạng Singleton | Đảm bảo `AppDbContext` và `ITenantProvider` được đăng ký dưới dạng Scoped | Ứng dụng khởi động bình thường |
| 6 | Async Linq ném exception khi chạy unit test | `IQueryable` từ `List<T>` thiếu async query provider | Sử dụng `MockQueryable.Moq` thông qua phương thức `.BuildMock()` | Toàn bộ Async Tests PASS |
| 7 | `DbUpdateConcurrencyException` khi lưu | Bản ghi đã bị sửa đổi đồng thời bởi giao dịch khác (`xmin` đổi) | Bắt ngoại lệ, tải lại dữ liệu mới nhất và yêu cầu xác nhận ghi đè | Đúng hành vi Concurrency $[K19]$ |
| 8 | `UserId` nhận giá trị `Guid.Empty` | Claim `NameIdentifier` không tồn tại trong Authentication State | Kiểm tra cấu hình xác thực ClaimsPrincipal trong Cookie/JWT | `ICurrentUser` lấy đúng định danh |
| 9 | Lỗi vi phạm khóa ngoại khi chạy DDL | Thứ tự tạo bảng sai lệch (bảng cha tạo sau bảng con) | Tạo bảng `teams`, `users` trước khi thêm FK vào `customers` | Script DDL chạy trơn tru |

---

## 8. BẢNG KIỂM ĐỊNH CUỐI — DoD 6 TIÊU CHÍ (FINAL AUDIT CHECKLIST — K48)

### ✅ Nghiệp vụ & Kỹ thuật
* [ ] DDL V1.0.2 thực thi thành công, idempotent (chạy 2 lần liên tiếp không lỗi).
* [ ] 4 Foreign Keys hiển thị đúng tên quy ước trong `pg_constraint`.
* [ ] Global Query Filter hoạt động chính xác cho cả Tenant và Soft Delete.
* [ ] Handlers xử lý CQRS phân tách rõ ràng; Query chỉ đọc dùng `.AsNoTracking()`.
* [ ] IDOR Lớp 2 ngăn chặn Sales truy cập hoặc cập nhật trái phép dữ liệu.

### ✅ Tiêu chuẩn chất lượng — DoD 6 tiêu chí (K48)
* [ ] **DoD-1:** `dotnet build -c Release` $\rightarrow$ **0 Warning, 0 Error**.
* [ ] **DoD-2:** Unit test bao phủ toàn diện, đạt **19/19 pass (100%)**.
* [ ] **DoD-3:** Ứng dụng chạy thử nghiệm trên môi trường cục bộ thành công (`dotnet run`).
* [ ] **DoD-4:** Hoàn tất cập nhật `data_dictionary.md` và `api_contracts.md`.
* [ ] **DoD-5:** Ghi nhận 3 mục nợ kỹ thuật vào `tech_debt_report.md`.
* [ ] **DoD-6:** Commit Git tuân thủ chuẩn Conventional Commits.

### ✅ Kiến trúc Model EF Core (K16, K17)
* [ ] Chạy `ModelConfigurationTests` đạt **5/5 pass**.
* [ ] Kiểm định đạt **0 shadow property** qua EF Core metadata API.
* [ ] $100\%$ quan hệ có tên ràng buộc tường minh qua `.HasConstraintName()`.