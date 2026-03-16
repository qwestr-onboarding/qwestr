namespace Qwestr.Shared.Contracts.Identity;

public interface IUserProvider
{
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<UserDto>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<List<UserDto>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default);
}