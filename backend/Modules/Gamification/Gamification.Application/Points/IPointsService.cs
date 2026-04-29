using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Points;

public interface IPointsService
{
    Task<ActionResult> AwardAsync(
        Guid tenantId,
        Guid profileId,
        int xp,
        int coins,
        PointsSourceType sourceType,
        Guid sourceId,
        string reason,
        CancellationToken ct);
}
