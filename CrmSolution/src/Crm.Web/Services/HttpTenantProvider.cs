using System;
using Crm.Domain.Common.Interfaces;

namespace Crm.Web.Services;

public sealed class HttpTenantProvider : ITenantProvider
{
    // Sprint 1 thiết lập tenant mặc định. Sprint 3+ đọc từ header hoặc JWT claim
    public Guid TenantId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
}
