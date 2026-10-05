using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Business.Customers.Dtos;
using Crm.Business.Customers.Queries;
using Crm.Data.Repositories;
using Crm.Domain.Entities;
using FluentAssertions;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Crm.Tests.Customers;

public class GetCustomerListQueryHandlerTests
{
    private static readonly Guid Sales1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Sales2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid TeamA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static GetCustomerListQueryHandler BuildSut()
    {
        var data = new List<Customer>
        {
            new() { Id=1, Code="KH-0001", Name="An Phát",   OwnerId=Sales1, TeamId=TeamA, HealthStatus=1, IsDeleted=false, CreatedAt=DateTime.UtcNow },
            new() { Id=2, Code="KH-0002", Name="Việt Nhật", OwnerId=Sales1, TeamId=TeamA, HealthStatus=1, IsDeleted=false, CreatedAt=DateTime.UtcNow },
            new() { Id=3, Code="KH-0003", Name="Tân Á",     OwnerId=Sales2, TeamId=TeamA, HealthStatus=2, IsDeleted=false, CreatedAt=DateTime.UtcNow },
        }.AsQueryable().BuildMock();

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
