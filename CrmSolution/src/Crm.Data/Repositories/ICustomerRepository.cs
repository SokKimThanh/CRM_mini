using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Domain.Entities;

namespace Crm.Data.Repositories;

public interface ICustomerRepository
{
    IQueryable<Customer> Query();
    Task<Customer?> GetByIdAsync(long id, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
    void Update(Customer customer);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
