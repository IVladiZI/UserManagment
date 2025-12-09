using UserManagement.Domain.Repositories;

namespace UserManagement.Infrastructure.Data;

public class UnitOfWork(UserManagementDbContext context) : IUnitOfWork
{
    private readonly UserManagementDbContext _context = context;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}