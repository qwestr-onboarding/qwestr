using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _context;

    public RefreshTokenRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, ct);

    public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => await _context.RefreshTokens.AddAsync(token, ct);

    public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.Revoke();
        }
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
