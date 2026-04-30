using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Application.Abstractions.Services;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Modules.Gamification.Domain.Events;
using Qwestr.Shared.Contracts.Gamification.Events;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Points;

public class PointsService : IPointsService
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IPointsTransactionRepository _transactionRepository;
    private readonly ILevelCalculator _levelCalculator;
    private readonly IPublisher _publisher;

    public PointsService(
        IPlayerProfileRepository profileRepository,
        IPointsTransactionRepository transactionRepository,
        ILevelCalculator levelCalculator,
        IPublisher publisher)
    {
        _profileRepository = profileRepository;
        _transactionRepository = transactionRepository;
        _levelCalculator = levelCalculator;
        _publisher = publisher;
    }

    public async Task<ActionResult> AwardAsync(
        Guid tenantId,
        Guid profileId,
        int xp,
        int coins,
        PointsSourceType sourceType,
        Guid sourceId,
        string reason,
        CancellationToken ct)
    {
        var existing = await _transactionRepository.GetByIdempotencyKeyAsync(profileId, sourceType, sourceId, ct);
        if (existing != null)
        {
            return ActionResult.Success();
        }

        var profile = await _profileRepository.GetByIdAsync(profileId, ct);
        if (profile == null)
        {
            return ActionResult.Failure("PointsService.ProfileNotFound");
        }

        if (xp > 0)
        {
            profile.AddXp(xp);
        }
        if (coins > 0)
        {
            profile.AddCoins(coins);
        }

        var transaction = PointsTransaction.Create(tenantId, profileId, xp, coins, sourceType, sourceId, reason);
        await _transactionRepository.AddAsync(transaction, ct);

        if (coins > 0)
        {
            var coinTx = CoinTransaction.CreateEarned(tenantId, profileId, coins, CoinTransactionSource.Earned, sourceId, reason);
            await _transactionRepository.AddCoinTransactionAsync(coinTx, ct);
        }

        var newLevel = _levelCalculator.CalculateLevel(profile.TotalXp);
        var oldLevel = profile.RecalculateLevel(newLevel);

        await _profileRepository.SaveChangesAsync(ct);

        if (xp > 0)
        {
            await _publisher.Publish(new PointsGrantedIntegrationEvent(
                tenantId, profile.UserId, xp, DateTime.UtcNow), ct);
        }

        if (oldLevel.HasValue)
        {
            await _publisher.Publish(new PlayerLevelUpEvent(tenantId, profileId, profile.UserId, oldLevel.Value, newLevel), ct);
        }

        return ActionResult.Success();
    }
}
