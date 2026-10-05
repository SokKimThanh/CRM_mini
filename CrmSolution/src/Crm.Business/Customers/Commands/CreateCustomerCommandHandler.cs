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
            Code = "PENDING",
            Name = cmd.Name.Trim(),
            Phone = cmd.Phone,
            Email = cmd.Email,
            TaxCode = cmd.TaxCode,
            Address = cmd.Address,
            Industry = cmd.Industry,
            HealthStatus = cmd.HealthStatus,
            OwnerId = _user.UserId,
            TeamId = _user.TeamId,
            TenantId = _tenant.TenantId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repo.AddAsync(customer, ct);
        await _repo.SaveChangesAsync(ct);

        // Sinh mã khách hàng 2-phase format
        customer.Code = $"KH-{customer.Id:D4}";
        await _repo.SaveChangesAsync(ct);

        return customer.Id;
    }
}
