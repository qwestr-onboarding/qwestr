using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Repositories;

public class ScenarioMetadataCacheRepository : IScenarioMetadataCacheRepository
{
    private readonly AnalyticsDbContext _db;

    public ScenarioMetadataCacheRepository(AnalyticsDbContext db)
    {
        _db = db;
    }

    public async Task<ScenarioMetadataCache?> FindByScenarioIdAsync(Guid tenantId, Guid scenarioId, CancellationToken ct)
    {
        return await _db.ScenarioMetadataCaches
            .Include(c => c.Stages)
            .ThenInclude(s => s.Steps)
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.ScenarioId == scenarioId, ct);
    }

    public async Task AddAsync(ScenarioMetadataCache cache, CancellationToken ct)
    {
        await _db.ScenarioMetadataCaches.AddAsync(cache, ct);
    }

    public Task UpdateAsync(ScenarioMetadataCache cache, CancellationToken ct)
    {
        _db.ScenarioMetadataCaches.Update(cache);
        return Task.CompletedTask;
    }

    public async Task DeleteByScenarioIdAsync(Guid tenantId, Guid scenarioId, CancellationToken ct)
    {
        await _db.ScenarioMetadataCaches
            .Where(c => c.TenantId == tenantId && c.ScenarioId == scenarioId)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetStepNamesAsync(
        Guid tenantId, IEnumerable<Guid> stepIds, CancellationToken ct)
    {
        var stepIdList = stepIds.ToList();
        if (stepIdList.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var result = await _db.StepMetadataItems
            .Where(s => stepIdList.Contains(s.StepId))
            .Select(s => new { s.StepId, s.StepName })
            .ToListAsync(ct);

        return result.ToDictionary(s => s.StepId, s => s.StepName);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetScenarioNamesAsync(
        Guid tenantId, IEnumerable<Guid> scenarioIds, CancellationToken ct)
    {
        var scenarioIdList = scenarioIds.ToList();
        if (scenarioIdList.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var result = await _db.ScenarioMetadataCaches
            .Where(c => c.TenantId == tenantId && scenarioIdList.Contains(c.ScenarioId))
            .Select(c => new { c.ScenarioId, c.ScenarioName })
            .ToListAsync(ct);

        return result.ToDictionary(c => c.ScenarioId, c => c.ScenarioName);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
