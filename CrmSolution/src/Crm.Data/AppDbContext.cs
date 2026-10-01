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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(Customer).Assembly);
    }
}