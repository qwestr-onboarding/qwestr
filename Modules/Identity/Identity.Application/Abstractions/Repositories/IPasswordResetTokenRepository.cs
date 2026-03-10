using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(PasswordResetToken token, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
