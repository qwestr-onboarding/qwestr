using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Abstractions.Repositories;

public interface IInvitationRepository
{
    Task<Invitation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Invitation?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<Invitation?> GetByTokenWithTenantAsync(string token, CancellationToken ct = default);
    Task<List<Invitation>> GetByTenantAsync(Guid tenantId, int page, int pageSize, InvitationStatus? status = null, CancellationToken ct = default);
    Task<int> CountByTenantAsync(Guid tenantId, InvitationStatus? status = null, CancellationToken ct = default);
    Task<bool> ExistsPendingAsync(Guid tenantId, string email, CancellationToken ct = default);
    Task AddAsync(Invitation invitation, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
