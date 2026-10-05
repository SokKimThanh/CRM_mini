using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Data.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db) => _db = db;

    public IQueryable<Customer> Query() => _db.Customers.AsQueryable();

    public Task<Customer?> GetByIdAsync(long id, CancellationToken ct)
        => _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Customer customer, CancellationToken ct)
        => await _db.Customers.AddAsync(customer, ct);

    public void Update(Customer customer) => _db.Customers.Update(customer);

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
