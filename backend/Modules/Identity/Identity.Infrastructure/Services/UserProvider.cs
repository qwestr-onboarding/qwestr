using Microsoft.EntityFrameworkCore;
using Qwestr.Shared.Contracts.Identity;

namespace Qwestr.Modules.Identity.Infrastructure.Services;

public class UserProvider : IUserProvider
{
    private readonly IdentityDbContext _context;

    public UserProvider(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDto(u.Id, u.TenantId, u.Email, u.FirstName, u.LastName, null, u.Role, u.IsActive))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<UserDto>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.Users
            .Where(u => u.TenantId == tenantId && u.IsActive)
            .Select(u => new UserDto(u.Id, u.TenantId, u.Email, u.FirstName, u.LastName, null, u.Role, u.IsActive))
            .ToListAsync(ct);
    }

    public async Task<List<UserDto>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default)
    {
        var ids = userIds.ToList();
        return await _context.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => new UserDto(u.Id, u.TenantId, u.Email, u.FirstName, u.LastName, null, u.Role, u.IsActive))
            .ToListAsync(ct);
    }
}