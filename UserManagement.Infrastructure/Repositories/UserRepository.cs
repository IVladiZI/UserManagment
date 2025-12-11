using Microsoft.EntityFrameworkCore;
using UserManagement.Domain;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository(UserManagementDbContext context) : IUserRepository
{
    private readonly UserManagementDbContext _context = context;

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<bool> ExistsByDocumentAsync(Document document, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Document.NumberDocument == document.NumberDocument, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}