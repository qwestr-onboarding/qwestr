using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Gamification.Application.Leaderboard.Queries.GetLeaderboard;
using Qwestr.Modules.Gamification.Application.Leaderboard.Queries.GetLeaderboardMyPosition;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Gamification.Api.Endpoints;

public static partial class GamificationEndpoints
{
    private static IEndpointRouteBuilder MapLeaderboardEndpoints(this IEndpointRouteBuilder gamification)
    {
        var leaderboard = gamification.MapGroup("leaderboard").WithTags("Gamification.Leaderboard").RequireAuthorization();

        leaderboard.MapGet("/", GetLeaderboardHandler);
        leaderboard.MapGet("/me", GetLeaderboardMyPositionHandler);

        return gamification;
    }

    private static async Task<IResult> GetLeaderboardHandler(
        [FromQuery] string? period, [FromQuery] int topN,
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var parsedPeriod = Enum.TryParse<LeaderboardPeriod>(period, true, out var p) ? p : LeaderboardPeriod.AllTime;
        var result = await sender.Send(new GetLeaderboardQuery(currentUser.TenantId, parsedPeriod, topN == 0 ? 50 : topN), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetLeaderboardMyPositionHandler(
        [FromQuery] string? period,
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var parsedPeriod = Enum.TryParse<LeaderboardPeriod>(period, true, out var p) ? p : LeaderboardPeriod.AllTime;
        var result = await sender.Send(new GetLeaderboardMyPositionQuery(currentUser.TenantId, currentUser.UserId, parsedPeriod), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }
}
