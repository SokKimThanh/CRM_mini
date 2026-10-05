using System;
using System.Collections.Generic;

namespace Crm.Business.Customers.Dtos;

public sealed class CustomerListDto
{
    public long Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Industry { get; init; }
    public string? Phone { get; init; }
    public int HealthStatus { get; init; }
    public decimal Revenue90d { get; init; }
    public Guid? OwnerId { get; init; }
}

public sealed class CustomerFilterDto
{
    public int? HealthStatus { get; init; }
    public Guid? OwnerId { get; init; }
    public string? Keyword { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class PagedResult<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
}
