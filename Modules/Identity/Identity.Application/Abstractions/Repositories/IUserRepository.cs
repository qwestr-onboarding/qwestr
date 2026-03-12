using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(Guid tenantId, string email, CancellationToken ct = default);
    Task<List<User>> GetActiveByTenantAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);

    Task<List<UserBackupCode>> GetActiveBackupCodesAsync(Guid userId, CancellationToken ct = default);
    Task AddBackupCodesAsync(IEnumerable<UserBackupCode> codes, CancellationToken ct = default);
    Task DeleteAllBackupCodesAsync(Guid userId, CancellationToken ct = default);
}
