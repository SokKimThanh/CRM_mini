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
