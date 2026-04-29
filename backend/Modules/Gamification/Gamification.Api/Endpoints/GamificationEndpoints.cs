using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Qwestr.Modules.Gamification.Api.Endpoints;

public static partial class GamificationEndpoints
{
    internal const string HrPolicy = "GamificationHrOnly";

    public static IEndpointRouteBuilder MapGamificationEndpoints(this IEndpointRouteBuilder app)
    {
        var gamification = app.MapGroup("/api/gamification");
        gamification.MapProfilesEndpoints();
        gamification.MapLevelTiersEndpoints();
        gamification.MapAwardsEndpoints();
        gamification.MapShopEndpoints();
        gamification.MapInventoryEndpoints();
        gamification.MapLeaderboardEndpoints();
        return app;
    }
}
