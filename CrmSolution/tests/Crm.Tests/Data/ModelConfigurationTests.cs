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
            .FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Crm.Domain.Entities.ApplicationUser));

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
        var fk = db.Model.FindEntityType(typeof(Crm.Domain.Entities.ApplicationUser))!
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
