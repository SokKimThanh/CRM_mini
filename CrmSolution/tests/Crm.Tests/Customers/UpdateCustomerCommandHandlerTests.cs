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
