using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Repositories;

public class PointsTransactionRepository : IPointsTransactionRepository
{
    private readonly GamificationDbContext _context;

    public PointsTransactionRepository(GamificationDbContext context)
    {
        _context = context;
    }

    public async Task<PointsTransaction?> GetByIdempotencyKeyAsync(Guid profileId, PointsSourceType sourceType, Guid sourceId, CancellationToken ct = default)
        => await _context.PointsTransactions.FirstOrDefaultAsync(
            t => t.ProfileId == profileId && t.SourceType == sourceType && t.SourceId == sourceId, ct);

    public async Task<List<PointsTransaction>> GetByProfileAsync(Guid profileId, DateTime? from, DateTime? to, int limit, CancellationToken ct = default)
    {
        var query = _context.PointsTransactions.Where(t => t.ProfileId == profileId);
        if (from.HasValue)
        {
            query = query.Where(t => t.OccurredAt >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(t => t.OccurredAt <= to.Value);
        }
        return await query
            .OrderByDescending(t => t.OccurredAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<List<(Guid ProfileId, int PeriodXp)>> GetTopByPeriodAsync(Guid tenantId, DateTime from, int topN, CancellationToken ct = default)
    {
        var result = await _context.PointsTransactions
            .Where(t => t.TenantId == tenantId && t.OccurredAt >= from)
            .GroupBy(t => t.ProfileId)
            .Select(g => new { ProfileId = g.Key, PeriodXp = g.Sum(t => t.XpAmount) })
            .OrderByDescending(r => r.PeriodXp)
            .Take(topN)
            .ToListAsync(ct);

        return result.Select(r => (r.ProfileId, r.PeriodXp)).ToList();
    }

    public async Task<int> GetPeriodXpForProfileAsync(Guid profileId, DateTime from, CancellationToken ct = default)
        => await _context.PointsTransactions
            .Where(t => t.ProfileId == profileId && t.OccurredAt >= from)
            .SumAsync(t => t.XpAmount, ct);

    public async Task AddAsync(PointsTransaction transaction, CancellationToken ct = default)
        => await _context.PointsTransactions.AddAsync(transaction, ct);

    public async Task AddCoinTransactionAsync(CoinTransaction transaction, CancellationToken ct = default)
        => await _context.CoinTransactions.AddAsync(transaction, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
