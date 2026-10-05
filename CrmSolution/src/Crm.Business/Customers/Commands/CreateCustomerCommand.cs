using MediatR;

namespace Crm.Business.Customers.Commands;

public sealed record CreateCustomerCommand(
    string Name, string? Phone, string? Email, string? TaxCode,
    string? Address, string? Industry, int HealthStatus) : IRequest<long>;
