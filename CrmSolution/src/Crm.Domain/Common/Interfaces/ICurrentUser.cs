using System;

namespace Crm.Domain.Common.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid? TeamId { get; }
    string Role { get; } // "SALES" | "MANAGER" | "ADMIN"
}
