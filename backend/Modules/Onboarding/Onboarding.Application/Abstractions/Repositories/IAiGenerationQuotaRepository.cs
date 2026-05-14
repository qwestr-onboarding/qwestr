using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;

public interface IAiGenerationQuotaRepository
{
    Task<AiGenerationQuota?> GetByTenantAndDateAsync(Guid tenantId, DateOnly date, CancellationToken ct = default);
    Task AddAsync(AiGenerationQuota quota, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
