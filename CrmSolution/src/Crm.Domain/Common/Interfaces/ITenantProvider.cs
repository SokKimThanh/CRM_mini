using System;

namespace Crm.Domain.Common.Interfaces;

public interface ITenantProvider
{
    Guid TenantId { get; }
}
