using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly IdentityDbContext _context;

    public PasswordResetTokenRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await _context.PasswordResetTokens.FirstOrDefaultAsync(p => p.Token == token, ct);

    public async Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await _context.PasswordResetTokens
            .Where(p => p.UserId == userId && !p.IsUsed)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.MarkAsUsed();
        }
    }

    public async Task AddAsync(PasswordResetToken token, CancellationToken ct = default)
        => await _context.PasswordResetTokens.AddAsync(token, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
