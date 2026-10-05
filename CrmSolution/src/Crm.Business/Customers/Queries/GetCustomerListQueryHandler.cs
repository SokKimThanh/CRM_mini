using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Business.Customers.Dtos;
using Crm.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crm.Business.Customers.Queries;

public sealed class GetCustomerListQueryHandler
    : IRequestHandler<GetCustomerListQuery, PagedResult<CustomerListDto>>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerListQueryHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<PagedResult<CustomerListDto>> Handle(
        GetCustomerListQuery request, CancellationToken ct)
    {
        var f = request.Filter;

        // [K18] Read-only tối ưu hóa bộ nhớ
        var query = _repo.Query().AsNoTracking();

        // [K39 - Lớp 2] IDOR: Phân quyền theo vai trò nghiệp vụ (Deny by default)
        query = request.CurrentRole switch
        {
            "SALES" => query.Where(c => c.OwnerId == request.CurrentUserId),
            "MANAGER" => query.Where(c => c.TeamId == request.CurrentTeamId),
            "ADMIN" => query,
            _ => query.Where(c => false)
        };

        if (f.HealthStatus.HasValue)
        {
            query = query.Where(c => c.HealthStatus == f.HealthStatus.Value);
        }

        if (f.OwnerId.HasValue)
        {
            query = query.Where(c => c.OwnerId == f.OwnerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(f.Keyword))
        {
            var kw = f.Keyword.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(kw) ||
                c.Code.ToLower().Contains(kw) ||
                (c.Phone != null && c.Phone.Contains(kw)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Skip((f.PageIndex - 1) * f.PageSize)
            .Take(f.PageSize)
            .Select(c => new CustomerListDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Industry = c.Industry,
                Phone = c.Phone,
                HealthStatus = c.HealthStatus,
                Revenue90d = c.Revenue90d ?? 0,
                OwnerId = c.OwnerId
            })
            .ToListAsync(ct);

        return new PagedResult<CustomerListDto>
        {
            Items = items,
            TotalCount = total,
            PageIndex = f.PageIndex,
            PageSize = f.PageSize
        };
    }
}
