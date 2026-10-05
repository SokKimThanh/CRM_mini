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

        customer.Name = cmd.Name.Trim();
        customer.Phone = cmd.Phone;
        customer.Email = cmd.Email;
        customer.TaxCode = cmd.TaxCode;
        customer.Address = cmd.Address;
        customer.Industry = cmd.Industry;
        customer.HealthStatus = cmd.HealthStatus;
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = _user.UserId;

        _repo.Update(customer);
        await _repo.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
