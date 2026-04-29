using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Abstractions.Persistence;

public interface IPointsTransactionRepository
{
    Task<PointsTransaction?> GetByIdempotencyKeyAsync(Guid profileId, PointsSourceType sourceType, Guid sourceId, CancellationToken ct = default);
    Task<List<PointsTransaction>> GetByProfileAsync(Guid profileId, DateTime? from, DateTime? to, int limit, CancellationToken ct = default);
    Task<List<(Guid ProfileId, int PeriodXp)>> GetTopByPeriodAsync(Guid tenantId, DateTime from, int topN, CancellationToken ct = default);
    Task<int> GetPeriodXpForProfileAsync(Guid profileId, DateTime from, CancellationToken ct = default);
    Task AddAsync(PointsTransaction transaction, CancellationToken ct = default);
    Task AddCoinTransactionAsync(CoinTransaction transaction, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
