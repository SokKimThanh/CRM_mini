using System;
using Crm.Business.Customers.Dtos;
using MediatR;

namespace Crm.Business.Customers.Queries;

public sealed record GetCustomerListQuery(
    CustomerFilterDto Filter,
    Guid CurrentUserId,
    Guid? CurrentTeamId,
    string CurrentRole) : IRequest<PagedResult<CustomerListDto>>;
