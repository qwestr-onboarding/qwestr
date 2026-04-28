using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class CoinTransaction : Entity<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid ProfileId { get; private set; }
    public int Amount { get; private set; }  // отрицательное при покупке
    public CoinTransactionSource Source { get; private set; }
    public Guid? SourceEntityId { get; private set; }  // ShopItemId при покупке, AwardId при бонусе
    public string Reason { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private CoinTransaction() { }

    public static CoinTransaction CreateEarned(Guid tenantId, Guid profileId, int amount, CoinTransactionSource source, Guid? sourceEntityId, string reason)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Earned amount must be positive");
        }
        return new CoinTransaction { /* ... */ };
    }

    public static CoinTransaction CreateSpent(Guid tenantId, Guid profileId, int amount, Guid shopItemId, string reason)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Spent amount must be positive (will be stored as negative)");
        }
        return new CoinTransaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProfileId = profileId,
            Amount = -amount,
            Source = CoinTransactionSource.Purchase,
            SourceEntityId = shopItemId,
            Reason = reason,
            OccurredAt = DateTime.UtcNow
        };
    }
}