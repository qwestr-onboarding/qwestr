using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Repositories;

public class AiGenerationQuotaRepository : IAiGenerationQuotaRepository
{
    private readonly OnboardingDbContext _db;

    public AiGenerationQuotaRepository(OnboardingDbContext db)
    {
        _db = db;
    }

    public Task<AiGenerationQuota?> GetByTenantAndDateAsync(Guid tenantId, DateOnly date, CancellationToken ct = default)
        => _db.AiGenerationQuotas
            .FirstOrDefaultAsync(q => q.TenantId == tenantId && q.Date == date, ct);

    public async Task AddAsync(AiGenerationQuota quota, CancellationToken ct = default)
        => await _db.AiGenerationQuotas.AddAsync(quota, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
