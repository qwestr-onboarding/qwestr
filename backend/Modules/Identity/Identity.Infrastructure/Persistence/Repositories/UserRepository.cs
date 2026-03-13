using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Email == email, ct);

    public async Task<bool> ExistsByEmailAsync(Guid tenantId, string email, CancellationToken ct = default)
        => await _context.Users.AnyAsync(u => u.TenantId == tenantId && u.Email == email, ct);

    public async Task<List<User>> GetActiveByTenantAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default)
        => await _context.Users
            .Where(u => u.TenantId == tenantId && u.IsActive)
            .OrderBy(u => u.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> CountByTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Users.CountAsync(u => u.TenantId == tenantId, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _context.Users.AddAsync(user, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public async Task<List<UserBackupCode>> GetActiveBackupCodesAsync(Guid userId, CancellationToken ct = default)
        => await _context.UserBackupCodes
            .Where(bc => bc.UserId == userId && !bc.IsUsed)
            .ToListAsync(ct);

    public async Task AddBackupCodesAsync(IEnumerable<UserBackupCode> codes, CancellationToken ct = default)
        => await _context.UserBackupCodes.AddRangeAsync(codes, ct);

    public async Task DeleteAllBackupCodesAsync(Guid userId, CancellationToken ct = default)
    {
        var codes = await _context.UserBackupCodes
            .Where(bc => bc.UserId == userId)
            .ToListAsync(ct);
        _context.UserBackupCodes.RemoveRange(codes);
    }
}
