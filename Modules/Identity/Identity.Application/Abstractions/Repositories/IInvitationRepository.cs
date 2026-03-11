using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Application.Abstractions.Repositories;

public interface IInvitationRepository
{
    Task<Invitation?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<List<Invitation>> GetByTenantAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
    Task<bool> ExistsPendingAsync(Guid tenantId, string email, CancellationToken ct = default);
    Task AddAsync(Invitation invitation, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
