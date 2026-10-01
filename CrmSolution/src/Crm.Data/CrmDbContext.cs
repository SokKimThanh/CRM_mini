using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Crm.Domain.Entities;

namespace Crm.Data;

public partial class CrmDbContext : DbContext
{
    public CrmDbContext()
    {
    }

    public CrmDbContext(DbContextOptions<CrmDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAssignment> CustomerAssignments { get; set; }

    public virtual DbSet<DashboardSnapshot> DashboardSnapshots { get; set; }

    public virtual DbSet<Interaction> Interactions { get; set; }

    public virtual DbSet<KiotvietSyncLog> KiotvietSyncLogs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Opportunity> Opportunities { get; set; }

    public virtual DbSet<OpportunityStage> OpportunityStages { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Quote> Quotes { get; set; }

    public virtual DbSet<QuoteItem> QuoteItems { get; set; }

    public virtual DbSet<SalesTask> SalesTasks { get; set; }

    public virtual DbSet<StageHistory> StageHistories { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_logs_pkey");

            entity.ToTable("audit_logs");

            entity.HasIndex(e => e.ChangedAt, "idx_audit_changed").IsDescending();

            entity.HasIndex(e => new { e.EntityName, e.EntityId }, "idx_audit_entity");

            entity.HasIndex(e => e.UserId, "idx_audit_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(20)
                .HasColumnName("action");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("changed_at");
            entity.Property(e => e.EntityId)
                .HasMaxLength(50)
                .HasColumnName("entity_id");
            entity.Property(e => e.EntityName)
                .HasMaxLength(100)
                .HasColumnName("entity_name");
            entity.Property(e => e.FieldName)
                .HasMaxLength(100)
                .HasColumnName("field_name");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.NewValuesJson)
                .HasColumnType("jsonb")
                .HasColumnName("new_values_json");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.OldValuesJson)
                .HasColumnType("jsonb")
                .HasColumnName("old_values_json");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(500)
                .HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.KiotvietId, "categories_kiotviet_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("display_order");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.KiotvietId).HasColumnName("kiotviet_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contacts_pkey");

            entity.ToTable("contacts");

            entity.HasIndex(e => e.CustomerId, "idx_contacts_customer");

            entity.HasIndex(e => new { e.CustomerId, e.IsPrimary }, "idx_contacts_primary").HasFilter("(is_primary = true)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValue(false)
                .HasColumnName("is_primary");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("contacts_customer_id_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.HasIndex(e => e.Code, "customers_code_key").IsUnique();

            entity.HasIndex(e => e.KiotvietId, "customers_kiotviet_id_key").IsUnique();

            entity.HasIndex(e => e.IsActive, "idx_customers_active").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.AssignedToUserId, "idx_customers_assigned").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.Code, "idx_customers_code");

            entity.HasIndex(e => e.HealthStatus, "idx_customers_health").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.KiotvietId, "idx_customers_kiotviet");

            entity.HasIndex(e => e.NextContactDue, "idx_customers_next_due").HasFilter("(is_deleted = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AssignedToUserId).HasColumnName("assigned_to_user_id");
            entity.Property(e => e.AverageCycleDays)
                .HasDefaultValue(0)
                .HasColumnName("average_cycle_days");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentDebt)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("current_debt");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.HealthStatus).HasColumnName("health_status");
            entity.Property(e => e.Industry)
                .HasMaxLength(100)
                .HasColumnName("industry");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.KiotvietId).HasColumnName("kiotviet_id");
            entity.Property(e => e.LastContactDate).HasColumnName("last_contact_date");
            entity.Property(e => e.LastOrderDate).HasColumnName("last_order_date");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NextContactDue).HasColumnName("next_contact_due");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.OrderCount90d)
                .HasDefaultValue(0)
                .HasColumnName("order_count_90d");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Revenue90d)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("revenue_90d");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(50)
                .HasColumnName("tax_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Website)
                .HasMaxLength(200)
                .HasColumnName("website");
        });

        modelBuilder.Entity<CustomerAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_assignments_pkey");

            entity.ToTable("customer_assignments");

            entity.HasIndex(e => e.CustomerId, "idx_assign_customer");

            entity.HasIndex(e => e.ToUserId, "idx_assign_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FromUserId).HasColumnName("from_user_id");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.ToUserId).HasColumnName("to_user_id");
            entity.Property(e => e.UnassignedAt).HasColumnName("unassigned_at");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerAssignments)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("customer_assignments_customer_id_fkey");
        });

        modelBuilder.Entity<DashboardSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dashboard_snapshots_pkey");

            entity.ToTable("dashboard_snapshots");

            entity.HasIndex(e => new { e.SnapshotDate, e.PeriodFrom, e.PeriodTo }, "dashboard_snapshots_snapshot_date_period_from_period_to_key").IsUnique();

            entity.HasIndex(e => e.SnapshotDate, "idx_snapshot_date").IsDescending();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertsJson)
                .HasColumnType("jsonb")
                .HasColumnName("alerts_json");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerRatioJson)
                .HasColumnType("jsonb")
                .HasColumnName("customer_ratio_json");
            entity.Property(e => e.FunnelJson)
                .HasColumnType("jsonb")
                .HasColumnName("funnel_json");
            entity.Property(e => e.MonthlyRevenueJson)
                .HasColumnType("jsonb")
                .HasColumnName("monthly_revenue_json");
            entity.Property(e => e.NewCustomers)
                .HasDefaultValue(0)
                .HasColumnName("new_customers");
            entity.Property(e => e.OldCustomers)
                .HasDefaultValue(0)
                .HasColumnName("old_customers");
            entity.Property(e => e.PeriodFrom).HasColumnName("period_from");
            entity.Property(e => e.PeriodTo).HasColumnName("period_to");
            entity.Property(e => e.SnapshotDate).HasColumnName("snapshot_date");
            entity.Property(e => e.TopStaffJson)
                .HasColumnType("jsonb")
                .HasColumnName("top_staff_json");
            entity.Property(e => e.TotalOppValue)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("total_opp_value");
            entity.Property(e => e.TotalOpportunities)
                .HasDefaultValue(0)
                .HasColumnName("total_opportunities");
            entity.Property(e => e.TotalRevenue)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("total_revenue");
            entity.Property(e => e.WinRate)
                .HasPrecision(5, 2)
                .HasDefaultValue(0m)
                .HasColumnName("win_rate");
            entity.Property(e => e.WonOpportunities)
                .HasDefaultValue(0)
                .HasColumnName("won_opportunities");
        });

        modelBuilder.Entity<Interaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("interactions_pkey");

            entity.ToTable("interactions");

            entity.HasIndex(e => e.CustomerId, "idx_interactions_customer");

            entity.HasIndex(e => e.InteractedAt, "idx_interactions_date").IsDescending();

            entity.HasIndex(e => e.UserId, "idx_interactions_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.InteractedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("interacted_at");
            entity.Property(e => e.OpportunityId).HasColumnName("opportunity_id");
            entity.Property(e => e.Outcome).HasColumnName("outcome");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Contact).WithMany(p => p.Interactions)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("interactions_contact_id_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Interactions)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("interactions_customer_id_fkey");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.Interactions)
                .HasForeignKey(d => d.OpportunityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("interactions_opportunity_id_fkey");
        });

        modelBuilder.Entity<KiotvietSyncLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("kiotviet_sync_logs_pkey");

            entity.ToTable("kiotviet_sync_logs");

            entity.HasIndex(e => e.Status, "idx_sync_logs_status");

            entity.HasIndex(e => new { e.EntityType, e.StartedAt }, "idx_sync_logs_type").IsDescending(false, true);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(20)
                .HasColumnName("action");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.DurationMs).HasColumnName("duration_ms");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.ErrorDetail).HasColumnName("error_detail");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.RecordsFailed)
                .HasDefaultValue(0)
                .HasColumnName("records_failed");
            entity.Property(e => e.RecordsSynced)
                .HasDefaultValue(0)
                .HasColumnName("records_synced");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.CreatedAt, "idx_notif_created").IsDescending();

            entity.HasIndex(e => new { e.RefType, e.RefId }, "idx_notif_ref");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "idx_notif_user_unread").HasFilter("(is_read = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(500)
                .HasColumnName("link_url");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Priority)
                .HasDefaultValue(1)
                .HasColumnName("priority");
            entity.Property(e => e.ReadAt).HasColumnName("read_at");
            entity.Property(e => e.RefId).HasColumnName("ref_id");
            entity.Property(e => e.RefType)
                .HasMaxLength(50)
                .HasColumnName("ref_type");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasDefaultValue(1)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Opportunity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("opportunities_pkey");

            entity.ToTable("opportunities");

            entity.HasIndex(e => e.AssignedToUserId, "idx_opp_assigned").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.CreatedAt, "idx_opp_created");

            entity.HasIndex(e => e.CustomerId, "idx_opp_customer");

            entity.HasIndex(e => e.ExpectedCloseDate, "idx_opp_expected");

            entity.HasIndex(e => e.StageId, "idx_opp_stage").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.IsWon, "idx_opp_won").HasFilter("(is_won = true)");

            entity.HasIndex(e => e.Code, "opportunities_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualCloseDate).HasColumnName("actual_close_date");
            entity.Property(e => e.AssignedToUserId).HasColumnName("assigned_to_user_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EstimatedValue)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("estimated_value");
            entity.Property(e => e.ExpectedCloseDate).HasColumnName("expected_close_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsWon).HasColumnName("is_won");
            entity.Property(e => e.LossNote).HasColumnName("loss_note");
            entity.Property(e => e.LossReason).HasColumnName("loss_reason");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Probability)
                .HasDefaultValue(0)
                .HasColumnName("probability");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .HasColumnName("source");
            entity.Property(e => e.StageId).HasColumnName("stage_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Contact).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("opportunities_contact_id_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("opportunities_customer_id_fkey");

            entity.HasOne(d => d.Stage).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.StageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("opportunities_stage_id_fkey");
        });

        modelBuilder.Entity<OpportunityStage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("opportunity_stages_pkey");

            entity.ToTable("opportunity_stages");

            entity.HasIndex(e => e.Name, "opportunity_stages_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(20)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DefaultProbability).HasColumnName("default_probability");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.IsClosed)
                .HasDefaultValue(false)
                .HasColumnName("is_closed");
            entity.Property(e => e.IsLost)
                .HasDefaultValue(false)
                .HasColumnName("is_lost");
            entity.Property(e => e.IsWon)
                .HasDefaultValue(false)
                .HasColumnName("is_won");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.HasIndex(e => e.IsActive, "idx_products_active").HasFilter("(is_active = true)");

            entity.HasIndex(e => e.CategoryId, "idx_products_category");

            entity.HasIndex(e => e.Code, "idx_products_code");

            entity.HasIndex(e => e.KiotvietId, "idx_products_kiotviet");

            entity.HasIndex(e => e.KiotvietId, "products_kiotviet_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("base_price");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CostPrice)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("cost_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.KiotvietId).HasColumnName("kiotviet_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("products_category_id_fkey");
        });

        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quotes_pkey");

            entity.ToTable("quotes");

            entity.HasIndex(e => e.Code, "idx_quotes_code");

            entity.HasIndex(e => e.CustomerId, "idx_quotes_customer").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.OpportunityId, "idx_quotes_opp");

            entity.HasIndex(e => e.QuoteDate, "idx_quotes_quote_date").IsDescending();

            entity.HasIndex(e => e.Status, "idx_quotes_status").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.Code, "quotes_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("discount_amount");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.OpportunityId).HasColumnName("opportunity_id");
            entity.Property(e => e.PaymentTerms).HasColumnName("payment_terms");
            entity.Property(e => e.QuoteDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("quote_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SubTotal)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("sub_total");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.ValidUntil).HasColumnName("valid_until");
            entity.Property(e => e.VatAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("vat_amount");
            entity.Property(e => e.VatRate)
                .HasPrecision(5, 2)
                .HasDefaultValue(10m)
                .HasColumnName("vat_rate");

            entity.HasOne(d => d.Contact).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("quotes_contact_id_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("quotes_customer_id_fkey");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.OpportunityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("quotes_opportunity_id_fkey");
        });

        modelBuilder.Entity<QuoteItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quote_items_pkey");

            entity.ToTable("quote_items");

            entity.HasIndex(e => e.ProductId, "idx_quote_items_product");

            entity.HasIndex(e => e.QuoteId, "idx_quote_items_quote");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("discount_amount");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(50)
                .HasColumnName("product_code");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity)
                .HasPrecision(12, 2)
                .HasColumnName("quantity");
            entity.Property(e => e.QuoteId).HasColumnName("quote_id");
            entity.Property(e => e.SortOrder)
                .HasDefaultValue(0)
                .HasColumnName("sort_order");
            entity.Property(e => e.TotalLine)
                .HasPrecision(18, 2)
                .HasColumnName("total_line");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Product).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("quote_items_product_id_fkey");

            entity.HasOne(d => d.Quote).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.QuoteId)
                .HasConstraintName("quote_items_quote_id_fkey");
        });

        modelBuilder.Entity<SalesTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sales_tasks_pkey");

            entity.ToTable("sales_tasks");

            entity.HasIndex(e => e.AssignedToUserId, "idx_tasks_assigned").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.CustomerId, "idx_tasks_customer").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.DueDate, "idx_tasks_due").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.OpportunityId, "idx_tasks_opp");

            entity.HasIndex(e => e.Status, "idx_tasks_status").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => new { e.AssignedToUserId, e.DueDate, e.Status }, "idx_tasks_today").HasFilter("((status = ANY (ARRAY[0, 1])) AND (is_deleted = false))");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedToUserId).HasColumnName("assigned_to_user_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CompletedBy).HasColumnName("completed_by");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.OpportunityId).HasColumnName("opportunity_id");
            entity.Property(e => e.Outcome).HasColumnName("outcome");
            entity.Property(e => e.Priority)
                .HasDefaultValue(1)
                .HasColumnName("priority");
            entity.Property(e => e.QuoteId).HasColumnName("quote_id");
            entity.Property(e => e.RelatedProductId).HasColumnName("related_product_id");
            entity.Property(e => e.ResultNote).HasColumnName("result_note");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasDefaultValue(6)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Contact).WithMany(p => p.SalesTasks)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sales_tasks_contact_id_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesTasks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("sales_tasks_customer_id_fkey");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.SalesTasks)
                .HasForeignKey(d => d.OpportunityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sales_tasks_opportunity_id_fkey");

            entity.HasOne(d => d.Quote).WithMany(p => p.SalesTasks)
                .HasForeignKey(d => d.QuoteId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sales_tasks_quote_id_fkey");

            entity.HasOne(d => d.RelatedProduct).WithMany(p => p.SalesTasks)
                .HasForeignKey(d => d.RelatedProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sales_tasks_related_product_id_fkey");
        });

        modelBuilder.Entity<StageHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("stage_histories_pkey");

            entity.ToTable("stage_histories");

            entity.HasIndex(e => e.OpportunityId, "idx_history_opp");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedByUserId).HasColumnName("changed_by_user_id");
            entity.Property(e => e.FromStageId).HasColumnName("from_stage_id");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.OpportunityId).HasColumnName("opportunity_id");
            entity.Property(e => e.ToStageId).HasColumnName("to_stage_id");

            entity.HasOne(d => d.FromStage).WithMany(p => p.StageHistoryFromStages)
                .HasForeignKey(d => d.FromStageId)
                .HasConstraintName("stage_histories_from_stage_id_fkey");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.StageHistories)
                .HasForeignKey(d => d.OpportunityId)
                .HasConstraintName("stage_histories_opportunity_id_fkey");

            entity.HasOne(d => d.ToStage).WithMany(p => p.StageHistoryToStages)
                .HasForeignKey(d => d.ToStageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stage_histories_to_stage_id_fkey");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("teams_pkey");

            entity.ToTable("teams");

            entity.HasIndex(e => e.ManagerId, "idx_teams_manager");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_profiles_pkey");

            entity.ToTable("user_profiles");

            entity.HasIndex(e => e.RoleCode, "idx_user_profiles_role");

            entity.HasIndex(e => e.TeamId, "idx_user_profiles_team");

            entity.HasIndex(e => e.EmployeeCode, "user_profiles_employee_code_key").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(20)
                .HasColumnName("employee_code");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MonthlyTarget)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("monthly_target");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleCode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'SALES'::character varying")
                .HasColumnName("role_code");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Team).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_profiles_team_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
