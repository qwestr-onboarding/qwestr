using Qwestr.Modules.Gamification.Application.Abstractions.Services;

namespace Qwestr.Modules.Gamification.Infrastructure.Services;

public class TenantConfigurationProvider : ITenantConfigurationProvider
{
    public Task<double> GetCoinsPerXpRateAsync(Guid tenantId, CancellationToken ct = default)
        => Task.FromResult(0.5);
}
