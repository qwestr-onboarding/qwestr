namespace Qwestr.Modules.Gamification.Application.Abstractions.Services;

public interface ITenantConfigurationProvider
{
    Task<double> GetCoinsPerXpRateAsync(Guid tenantId, CancellationToken ct = default);
}
