using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
