# =====================================================================
# SETUP DAY 3 + DAY 4 — HOÀN CHỈNH
# Chạy: .\scripts\setup-day3-4.ps1
# =====================================================================

param(
    [string]$ConnStr = $env:CRM_CONNECTION_STRING,
    [string]$PgUser = "crm_user",
    [string]$PgPass = "sa",
    [string]$PgDb   = "crm_db"
)

$ErrorActionPreference = "Stop"
if (-not $ConnStr) { $ConnStr = "Host=localhost;Port=5432;Database=$PgDb;Username=$PgUser;Password=$PgPass" }

$root        = Split-Path -Parent $PSScriptRoot
$dataPath    = Join-Path $root "src\Crm.Data"
$domainPath  = Join-Path $root "src\Crm.Domain\Entities"
$configPath  = Join-Path $root "src\Crm.Domain\Configurations"
$domainProj  = Join-Path $root "src\Crm.Domain"
$webPath     = Join-Path $root "src\Crm.Web"
$docsDb      = Join-Path $root "docs\db"
$tempDir     = Join-Path $dataPath "ScaffoldTemp"
$programPath = Join-Path $webPath "Program.cs"
$seedPath    = Join-Path $docsDb "04_demo_data.sql"

function Write-Utf8NoBom {
    param([string]$Path, [string]$Content)
    $enc = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($Path, $Content, $enc)
}

function Add-PackageIfMissing {
    param([string]$ProjectDir, [string]$CsprojFile, [string]$PackageName, [string]$Version = "10.0.0")
    Push-Location $ProjectDir
    $csproj = Get-Content $CsprojFile -Raw
    $regexPattern = $PackageName -replace '\.', '\.'
    if ($csproj -notmatch $regexPattern) {
        Write-Host "  + $PackageName" -ForegroundColor Green
        dotnet add package $PackageName --version $Version | Out-Null
        if ($LASTEXITCODE -ne 0) { Pop-Location; throw "Cài $PackageName failed" }
    }
    Pop-Location
}

# Trích body của modelBuilder.Entity<X>(entity => { BODY });
# Trả về ARRAY (không bị unwrap bởi pipeline)
function Extract-EntityBlocks {
    param([string]$Content)
    $result = @()
    $pattern = 'modelBuilder\.Entity<(\w+)>\(entity\s*=>\s*\{'
    $regex = [regex]$pattern
    foreach ($m in $regex.Matches($Content)) {
        $entityName = $m.Groups[1].Value
        $bodyStart = $m.Index + $m.Length
        $depth = 1
        $i = $bodyStart
        while ($i -lt $Content.Length -and $depth -gt 0) {
            $ch = $Content[$i]
            if ($ch -eq '{') { $depth++ }
            elseif ($ch -eq '}') { $depth--; if ($depth -eq 0) { break } }
            $i++
        }
        $body = $Content.Substring($bodyStart, $i - $bodyStart)
        $result += [PSCustomObject]@{ EntityName = $entityName; Body = $body }
    }
    return ,$result
}

Write-Host "=== SETUP DAY 3 + DAY 4 ===" -ForegroundColor Cyan

# =====================================================================
# [1] PREREQUISITES
# =====================================================================
Write-Host "`n[1/9] Prerequisites..." -ForegroundColor Yellow
if (-not (Test-Path $dataPath))   { throw "Không tìm thấy src\Crm.Data" }
if (-not (Test-Path $domainPath)) { throw "Không tìm thấy src\Crm.Domain\Entities" }
if (-not (Get-Command dotnet-ef -ErrorAction SilentlyContinue)) { dotnet tool install --global dotnet-ef }
if (-not (Get-Command psql -ErrorAction SilentlyContinue)) { throw "Thiếu psql" }

$env:PGPASSWORD = $PgPass
$null = psql -h localhost -U $PgUser -d $PgDb -t -c "SELECT 1;" 2>&1
if ($LASTEXITCODE -ne 0) { throw "Không kết nối DB $PgDb" }

New-Item -ItemType Directory -Force -Path $docsDb, $domainPath, $configPath | Out-Null
Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [2] SQL Day 3 — Identity tables + user_profiles + teams + FK
# =====================================================================
Write-Host "`n[2/9] SQL Day 3..." -ForegroundColor Yellow

$sql = @'
DO $$ BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='customers' AND column_name='assigned_to_user_id' AND data_type='character varying') THEN ALTER TABLE customers ALTER COLUMN assigned_to_user_id TYPE UUID USING NULL; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='opportunities' AND column_name='assigned_to_user_id' AND data_type='character varying') THEN ALTER TABLE opportunities ALTER COLUMN assigned_to_user_id TYPE UUID USING NULL; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='sales_tasks' AND column_name='assigned_to_user_id' AND data_type='character varying') THEN ALTER TABLE sales_tasks ALTER COLUMN assigned_to_user_id TYPE UUID USING NULL; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='interactions' AND column_name='user_id' AND data_type='character varying') THEN ALTER TABLE interactions ALTER COLUMN user_id TYPE UUID USING NULL; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='quotes' AND column_name='created_by_user_id' AND data_type='character varying') THEN ALTER TABLE quotes ALTER COLUMN created_by_user_id TYPE UUID USING NULL; END IF;
END $$;

CREATE TABLE IF NOT EXISTS "AspNetRoles" ("Id" UUID PRIMARY KEY, "Name" VARCHAR(256), "NormalizedName" VARCHAR(256), "ConcurrencyStamp" TEXT);
CREATE UNIQUE INDEX IF NOT EXISTS "RoleNameIndex" ON "AspNetRoles" ("NormalizedName") WHERE "NormalizedName" IS NOT NULL;
CREATE TABLE IF NOT EXISTS "AspNetUsers" (
    "Id" UUID PRIMARY KEY, "UserName" VARCHAR(256), "NormalizedUserName" VARCHAR(256),
    "Email" VARCHAR(256), "NormalizedEmail" VARCHAR(256), "EmailConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "PasswordHash" TEXT, "SecurityStamp" TEXT, "ConcurrencyStamp" TEXT, "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL DEFAULT FALSE, "TwoFactorEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "LockoutEnd" TIMESTAMPTZ, "LockoutEnabled" BOOLEAN NOT NULL DEFAULT FALSE, "AccessFailedCount" INT NOT NULL DEFAULT 0);
CREATE UNIQUE INDEX IF NOT EXISTS "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName") WHERE "NormalizedUserName" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
CREATE TABLE IF NOT EXISTS "AspNetUserRoles" ("UserId" UUID NOT NULL REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE, "RoleId" UUID NOT NULL REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE, PRIMARY KEY ("UserId","RoleId"));
CREATE TABLE IF NOT EXISTS "AspNetUserClaims" ("Id" SERIAL PRIMARY KEY, "UserId" UUID NOT NULL REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE, "ClaimType" TEXT, "ClaimValue" TEXT);
CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
CREATE TABLE IF NOT EXISTS "AspNetRoleClaims" ("Id" SERIAL PRIMARY KEY, "RoleId" UUID NOT NULL REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE, "ClaimType" TEXT, "ClaimValue" TEXT);
CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");
CREATE TABLE IF NOT EXISTS "AspNetUserLogins" ("LoginProvider" VARCHAR(128) NOT NULL, "ProviderKey" VARCHAR(128) NOT NULL, "ProviderDisplayName" TEXT, "UserId" UUID NOT NULL REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE, PRIMARY KEY ("LoginProvider","ProviderKey"));
CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
CREATE TABLE IF NOT EXISTS "AspNetUserTokens" ("UserId" UUID NOT NULL REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE, "LoginProvider" VARCHAR(128) NOT NULL, "Name" VARCHAR(128) NOT NULL, "Value" TEXT, PRIMARY KEY ("UserId","LoginProvider","Name"));

CREATE OR REPLACE FUNCTION update_updated_at_column() RETURNS TRIGGER AS $$ BEGIN NEW.updated_at = NOW(); RETURN NEW; END; $$ LANGUAGE plpgsql;

CREATE TABLE IF NOT EXISTS teams (
    id SERIAL PRIMARY KEY, name VARCHAR(100) NOT NULL,
    manager_id UUID REFERENCES "AspNetUsers"("Id") ON DELETE SET NULL,
    is_active BOOLEAN DEFAULT TRUE, created_at TIMESTAMPTZ DEFAULT NOW());
CREATE INDEX IF NOT EXISTS idx_teams_manager ON teams(manager_id);

CREATE TABLE IF NOT EXISTS user_profiles (
    user_id UUID PRIMARY KEY REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE,
    full_name VARCHAR(100) NOT NULL, employee_code VARCHAR(20) UNIQUE,
    phone VARCHAR(20), avatar_url VARCHAR(500), role_code VARCHAR(20) NOT NULL DEFAULT 'SALES',
    team_id INT REFERENCES teams(id) ON DELETE SET NULL, monthly_target NUMERIC(18,2) DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE, created_at TIMESTAMPTZ DEFAULT NOW(), updated_at TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_role_code CHECK (role_code IN ('ADMIN','MANAGER','SALES','ACCOUNTANT')));
CREATE INDEX IF NOT EXISTS idx_user_profiles_role ON user_profiles(role_code);
CREATE INDEX IF NOT EXISTS idx_user_profiles_team ON user_profiles(team_id);
DROP TRIGGER IF EXISTS trg_user_profiles_updated_at ON user_profiles;
CREATE TRIGGER trg_user_profiles_updated_at BEFORE UPDATE ON user_profiles FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DO $$ BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_customers_user') THEN ALTER TABLE customers ADD CONSTRAINT fk_customers_user FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id") ON DELETE SET NULL; END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_opp_user') THEN ALTER TABLE opportunities ADD CONSTRAINT fk_opp_user FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id") ON DELETE SET NULL; END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_tasks_user') THEN ALTER TABLE sales_tasks ADD CONSTRAINT fk_tasks_user FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id") ON DELETE RESTRICT; END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_interactions_user') THEN ALTER TABLE interactions ADD CONSTRAINT fk_interactions_user FOREIGN KEY (user_id) REFERENCES "AspNetUsers"("Id") ON DELETE RESTRICT; END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_quotes_user') THEN ALTER TABLE quotes ADD CONSTRAINT fk_quotes_user FOREIGN KEY (created_by_user_id) REFERENCES "AspNetUsers"("Id") ON DELETE RESTRICT; END IF;
END $$;
'@

$sqlPath = Join-Path $docsDb "setup-identity.sql"
Write-Utf8NoBom -Path $sqlPath -Content $sql
psql -h localhost -U $PgUser -d $PgDb -P pager=off -f $sqlPath
if ($LASTEXITCODE -ne 0) { throw "SQL Day 3 failed" }
Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [3] 3 ENTITY VIẾT TAY
# =====================================================================
Write-Host "`n[3/9] 3 entity viết tay..." -ForegroundColor Yellow

Write-Utf8NoBom -Path (Join-Path $domainPath "ApplicationUser.cs") -Content @'
using Microsoft.AspNetCore.Identity;
namespace Crm.Domain.Entities;
public class ApplicationUser : IdentityUser<Guid> { }
'@

Write-Utf8NoBom -Path (Join-Path $domainPath "UserProfile.cs") -Content @'
namespace Crm.Domain.Entities;
public class UserProfile
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string RoleCode { get; set; } = "SALES";
    public int? TeamId { get; set; }
    public decimal MonthlyTarget { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; } = null!;
    public Team? Team { get; set; }
}
'@

Write-Utf8NoBom -Path (Join-Path $domainPath "Team.cs") -Content @'
namespace Crm.Domain.Entities;
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
'@

Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [4] PACKAGE CRM.DOMAIN
# =====================================================================
Write-Host "`n[4/9] Package Crm.Domain..." -ForegroundColor Yellow
Add-PackageIfMissing -ProjectDir $domainProj -CsprojFile "Crm.Domain.csproj" -PackageName "Microsoft.Extensions.Identity.Stores"
Add-PackageIfMissing -ProjectDir $domainProj -CsprojFile "Crm.Domain.csproj" -PackageName "Microsoft.EntityFrameworkCore"
Add-PackageIfMissing -ProjectDir $domainProj -CsprojFile "Crm.Domain.csproj" -PackageName "Microsoft.EntityFrameworkCore.Relational"
Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [4.5] PACKAGE CRM.DATA
# =====================================================================
Write-Host "`n[4.5/9] Package Crm.Data..." -ForegroundColor Yellow
Add-PackageIfMissing -ProjectDir $dataPath -CsprojFile "Crm.Data.csproj" -PackageName "Microsoft.AspNetCore.Identity.EntityFrameworkCore"
Add-PackageIfMissing -ProjectDir $dataPath -CsprojFile "Crm.Data.csproj" -PackageName "Npgsql.EntityFrameworkCore.PostgreSQL"
Add-PackageIfMissing -ProjectDir $dataPath -CsprojFile "Crm.Data.csproj" -PackageName "Microsoft.EntityFrameworkCore.Design"
Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [5] XÓA CŨ + SCAFFOLD 16 ENTITY
# =====================================================================
Write-Host "`n[5/9] Xóa cũ + Scaffold..." -ForegroundColor Yellow

foreach ($f in @("AppDbContext.cs","AppIdentityDbContext.cs","CrmDbContext.cs","DbSeeder.cs")) {
    $p = Join-Path $dataPath $f
    if (Test-Path $p) { Remove-Item $p -Force }
}
if (Test-Path $tempDir) { Remove-Item $tempDir -Recurse -Force }

$keep = @("ApplicationUser.cs","UserProfile.cs","Team.cs")
Get-ChildItem "$domainPath\*.cs" | Where-Object { $keep -notcontains $_.Name } | Remove-Item -Force
if (Test-Path $configPath) { Get-ChildItem "$configPath\*.cs" | Remove-Item -Force }

$tables = @("customers","contacts","customer_assignments","opportunities","opportunity_stages","stage_histories","categories","products","quotes","quote_items","sales_tasks","interactions","notifications","dashboard_snapshots","audit_logs","kiotviet_sync_logs")
$tableArgs = foreach ($t in $tables) { "--table"; $t }

Push-Location $dataPath
dotnet ef dbcontext scaffold $ConnStr Npgsql.EntityFrameworkCore.PostgreSQL `
    --output-dir ScaffoldTemp --context-dir ScaffoldTemp --context ScaffoldTempContext `
    --no-onconfiguring --data-annotations @tableArgs
$exit = $LASTEXITCODE
Pop-Location
if ($exit -ne 0) { throw "Scaffold failed" }

Get-ChildItem "$tempDir\*.cs" | Where-Object { $_.Name -ne "ScaffoldTempContext.cs" } | ForEach-Object {
    $dest = Join-Path $domainPath $_.Name
    Copy-Item $_.FullName $dest -Force
    $content = [System.IO.File]::ReadAllText($dest) -replace 'namespace Crm\.Data\.ScaffoldTemp', 'namespace Crm.Domain.Entities'
    Write-Utf8NoBom -Path $dest -Content $content
}
Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [6] CONVERT SCAFFOLD CONFIG → IEntityTypeConfiguration<T>
# =====================================================================
Write-Host "`n[6/9] Convert configs..." -ForegroundColor Yellow

$tempCtx = Get-Content (Join-Path $tempDir "ScaffoldTempContext.cs") -Raw

# Extract DbSet list
$dbsetMatches = [regex]::Matches($tempCtx, 'public\s+virtual\s+DbSet<(\w+)>\s+(\w+)\s*\{')
if ($dbsetMatches.Count -lt 16) { throw "Chỉ có $($dbsetMatches.Count)/16 DbSet" }

# Extract entity config blocks
$blocks = Extract-EntityBlocks -Content $tempCtx
$blocks = $blocks | Where-Object { $_.Body.Trim().Length -gt 0 }

# Filter bỏ Team/UserProfile (sẽ viết tay riêng)
$bizBlocks = $blocks | Where-Object { $_.EntityName -notin @("Team","UserProfile") }
if ($bizBlocks.Count -lt 16) { throw "Chỉ extract được $($bizBlocks.Count)/16 business config block" }

# Sinh config cho từng business entity
$generated = 0
foreach ($b in $bizBlocks) {
    # Bỏ HasConstraintName để tránh warning khi EF Core validate
    $cleanBody = $b.Body

    $configContent = @"
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class $($b.EntityName)Configuration : IEntityTypeConfiguration<$($b.EntityName)>
{
    public void Configure(EntityTypeBuilder<$($b.EntityName)> entity)
    {
$cleanBody
    }
}
"@
    Write-Utf8NoBom -Path (Join-Path $configPath "$($b.EntityName)Configuration.cs") -Content $configContent
    $generated++
}
Write-Host "  → $generated business config files" -ForegroundColor Gray

# Viết tay 2 config cho Team + UserProfile
Write-Utf8NoBom -Path (Join-Path $configPath "UserProfileConfiguration.cs") -Content @'
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> entity)
    {
        entity.ToTable("user_profiles");
        entity.HasKey(u => u.UserId);
        entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
        entity.Property(u => u.EmployeeCode).HasMaxLength(20);
        entity.Property(u => u.RoleCode).IsRequired().HasMaxLength(20);
        entity.HasOne(u => u.User).WithOne()
            .HasForeignKey<UserProfile>(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(u => u.Team).WithMany(t => t.UserProfiles)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
'@

Write-Utf8NoBom -Path (Join-Path $configPath "TeamConfiguration.cs") -Content @'
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> entity)
    {
        entity.ToTable("teams");
        entity.HasKey(t => t.Id);
        entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
        entity.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(t => t.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
'@

# Sinh AppDbContext gọn
$dbsetsBlock = ($dbsetMatches | ForEach-Object { "    public DbSet<$($_.Groups[1].Value)> $($_.Groups[2].Value) => Set<$($_.Groups[1].Value)>();" }) -join "`r`n"

$dbContextTemplate = @"
using Crm.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Crm.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Team> Teams => Set<Team>();

$dbsetsBlock

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(Customer).Assembly);
    }
}
"@

Write-Utf8NoBom -Path (Join-Path $dataPath "AppDbContext.cs") -Content $dbContextTemplate

Remove-Item $tempDir -Recurse -Force
Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [7] DBSEDER
# =====================================================================
Write-Host "`n[7/9] DbSeeder.cs..." -ForegroundColor Yellow

Write-Utf8NoBom -Path (Join-Path $dataPath "DbSeeder.cs") -Content @'
using Crm.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crm.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, bool seedDemoData = false)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var dbContext   = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger      = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        foreach (var roleName in new[] { "ADMIN", "MANAGER", "SALES", "ACCOUNTANT" })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName });
                logger.LogInformation("Created role: {Role}", roleName);
            }
        }

        const string adminEmail = "admin@crm.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser { Id = Guid.NewGuid(), UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, LockoutEnabled = true };
            var r = await userManager.CreateAsync(admin, "Admin@2026");
            if (r.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "ADMIN");
                dbContext.UserProfiles.Add(new UserProfile { UserId = admin.Id, FullName = "System Administrator", EmployeeCode = "ADM001", RoleCode = "ADMIN", IsActive = true });
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Created admin: {Email}", adminEmail);
            }
        }

        if (seedDemoData)
        {
            var demos = new[]
            {
                new { Email = "sales1@crm.local",     Password = "Sales@2026",   Name = "Nguyễn Văn A", Code = "SAL001", Role = "SALES" },
                new { Email = "sales2@crm.local",     Password = "Sales@2026",   Name = "Trần Thị B",   Code = "SAL002", Role = "SALES" },
                new { Email = "manager@crm.local",    Password = "Manager@2026", Name = "Lê Văn C",     Code = "MGR001", Role = "MANAGER" },
                new { Email = "accountant@crm.local", Password = "Acc@2026",     Name = "Phạm Thị D",   Code = "ACC001", Role = "ACCOUNTANT" }
            };
            foreach (var u in demos)
            {
                if (await userManager.FindByEmailAsync(u.Email) != null) continue;
                var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = u.Email, Email = u.Email, EmailConfirmed = true, LockoutEnabled = true };
                var r = await userManager.CreateAsync(user, u.Password);
                if (r.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, u.Role);
                    dbContext.UserProfiles.Add(new UserProfile { UserId = user.Id, FullName = u.Name, EmployeeCode = u.Code, RoleCode = u.Role, IsActive = true });
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
'@

Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [8] CHECK + BUILD
# =====================================================================
Write-Host "`n[8/9] Check + Build..." -ForegroundColor Yellow

if (Test-Path $programPath) {
    $prog = Get-Content $programPath -Raw
    if ($prog -notmatch 'AddDbContext<AppDbContext>') { Write-Host "  [!] Program.cs thiếu AddDbContext<AppDbContext>" -ForegroundColor Red }
    if ($prog -notmatch 'DbSeeder\.SeedAsync') { Write-Host "  [!] Program.cs thiếu DbSeeder.SeedAsync" -ForegroundColor Red }
}

Push-Location $root
dotnet restore
if ($LASTEXITCODE -ne 0) { Pop-Location; throw "Restore failed" }
dotnet build
$buildExit = $LASTEXITCODE
Pop-Location
if ($buildExit -ne 0) { throw "Build failed" }
Write-Host "  OK" -ForegroundColor Green

# =====================================================================
# [9] SEED DEMO DATA
# =====================================================================
Write-Host "`n[9/9] Seed demo data..." -ForegroundColor Yellow

if (Test-Path $seedPath) {
    chcp 65001 | Out-Null
    [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
    $env:PGCLIENTENCODING = "UTF8"
    psql -h localhost -U $PgUser -d $PgDb -P pager=off -f $seedPath
    if ($LASTEXITCODE -ne 0) { throw "Seed failed" }
    $c = (psql -h localhost -U $PgUser -d $PgDb -t -c "SELECT COUNT(*) FROM customers;").Trim()
    $h = (psql -h localhost -U $PgUser -d $PgDb -t -c "SELECT COUNT(*) FROM stage_histories;").Trim()
    Write-Host "  Verify: customers=$c, stage_histories=$h" -ForegroundColor Green
}

Write-Host "`n=== HOÀN TẤT ===" -ForegroundColor Green
Write-Host "Bước cuối: cd src\Crm.Web; dotnet run" -ForegroundColor White
Write-Host "Login: admin@crm.local / Admin@2026" -ForegroundColor White
