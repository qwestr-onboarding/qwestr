using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

/// <summary>
/// Дневной агрегированный срез активности по тенанту.
/// Создаётся фоновым джобом раз в сутки и используется для построения
/// тренд-графиков на дашборде без агрегации сырых фактов в момент запроса.
/// Допускает пересчёт за тот же день — например, при поздно пришедшем событии.
/// </summary>
public class DailyEngagementSnapshot : Entity<Guid>
{
    public Guid TenantId { get; private set; }
    public DateOnly Date { get; private set; }
    public int ActiveUsers { get; private set; }
    public int StepsCompleted { get; private set; }
    public int ScenariosStarted { get; private set; }
    public int ScenariosCompleted { get; private set; }
    public int BadgesEarned { get; private set; }
    public int TotalPointsGranted { get; private set; }
    public DateTime CalculatedAt { get; private set; }

    // EF Core
    private DailyEngagementSnapshot() { }

    public static ActionResult<DailyEngagementSnapshot> Create(
        Guid tenantId, DateOnly date,
        int activeUsers, int stepsCompleted, int scenariosStarted,
        int scenariosCompleted, int badgesEarned, int totalPointsGranted,
        DateTime calculatedAt)
    {
        if (tenantId == Guid.Empty)
        {
            return ActionResult<DailyEngagementSnapshot>.Failure("analytics.snapshot.tenant_required");
        }

        if (activeUsers < 0 || stepsCompleted < 0 || scenariosStarted < 0
            || scenariosCompleted < 0 || badgesEarned < 0 || totalPointsGranted < 0)
        {
            return ActionResult<DailyEngagementSnapshot>.Failure("analytics.snapshot.metrics_negative");
        }

        var snapshot = new DailyEngagementSnapshot
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Date = date,
            ActiveUsers = activeUsers,
            StepsCompleted = stepsCompleted,
            ScenariosStarted = scenariosStarted,
            ScenariosCompleted = scenariosCompleted,
            BadgesEarned = badgesEarned,
            TotalPointsGranted = totalPointsGranted,
            CalculatedAt = calculatedAt
        };

        return ActionResult<DailyEngagementSnapshot>.Success(snapshot);
    }

    /// <summary>
    /// Полностью переписать значения метрик. Используется при пересчёте дневного среза
    /// фоновым джобом. Дата и тенант остаются неизменными.
    /// </summary>
    public ActionResult Recalculate(
        int activeUsers, int stepsCompleted, int scenariosStarted,
        int scenariosCompleted, int badgesEarned, int totalPointsGranted,
        DateTime calculatedAt)
    {
        if (activeUsers < 0 || stepsCompleted < 0 || scenariosStarted < 0
            || scenariosCompleted < 0 || badgesEarned < 0 || totalPointsGranted < 0)
        {
            return ActionResult.Failure("analytics.snapshot.metrics_negative");
        }

        ActiveUsers = activeUsers;
        StepsCompleted = stepsCompleted;
        ScenariosStarted = scenariosStarted;
        ScenariosCompleted = scenariosCompleted;
        BadgesEarned = badgesEarned;
        TotalPointsGranted = totalPointsGranted;
        CalculatedAt = calculatedAt;

        return ActionResult.Success();
    }
}