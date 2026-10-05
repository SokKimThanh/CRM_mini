using System;
using Crm.Domain.Common.Interfaces;
using Crm.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Crm.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly Guid _currentTenantId;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenant)
        : base(options)
    {
        _currentTenantId = tenant.TenantId;
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAssignment> CustomerAssignments => Set<CustomerAssignment>();
    public DbSet<DashboardSnapshot> DashboardSnapshots => Set<DashboardSnapshot>();
    public DbSet<Interaction> Interactions => Set<Interaction>();
    public DbSet<KiotvietSyncLog> KiotvietSyncLogs => Set<KiotvietSyncLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<OpportunityStage> OpportunityStages => Set<OpportunityStage>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();
    public DbSet<SalesTask> SalesTasks => Set<SalesTask>();
    public DbSet<StageHistory> StageHistories => Set<StageHistory>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        mb.ApplyConfigurationsFromAssembly(typeof(Customer).Assembly);

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
        mb.Entity<ApplicationUser>()
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

        mb.Entity<ApplicationUser>()
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
    }
}
