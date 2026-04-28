using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class AchievementProgress : Entity<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid ProfileId { get; private set; }
    public Guid AwardId { get; private set; }
    public int CurrentValue { get; private set; }
    public int TargetValue { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private AchievementProgress() { }

    public static AchievementProgress Create(Guid tenantId, Guid profileId, Guid awardId, int targetValue)
    {
        return new AchievementProgress
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProfileId = profileId,
            AwardId = awardId,
            CurrentValue = 0,
            TargetValue = targetValue,
            IsCompleted = false
        };
    }

    /// <summary>
    /// Инкрементирует прогресс. Возвращает true, если перешёл в IsCompleted в ходе этого вызова.
    /// </summary>
    public bool Increment(int delta)
    {
        if (IsCompleted || delta <= 0)
        {
            return false;
        }
        CurrentValue = Math.Min(CurrentValue + delta, TargetValue);
        if (CurrentValue >= TargetValue)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Устанавливает значение напрямую (для Threshold-условий типа «уровень 5» — там не инкремент, а проверка).
    /// </summary>
    public bool SetValue(int value)
    {
        if (IsCompleted || value < CurrentValue)
        {
            return false;
        }
        CurrentValue = Math.Min(value, TargetValue);
        if (CurrentValue >= TargetValue)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }
}