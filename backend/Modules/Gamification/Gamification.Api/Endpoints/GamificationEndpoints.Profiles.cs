using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyAchievementProgress;
using Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyEarnedAwards;
using Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyProfile;
using Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyTransactions;
using Qwestr.Modules.Gamification.Application.Profiles.Queries.GetProfileByUserId;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Gamification.Api.Endpoints;

public static partial class GamificationEndpoints
{
    private static IEndpointRouteBuilder MapProfilesEndpoints(this IEndpointRouteBuilder gamification)
    {
        var profiles = gamification.MapGroup("profiles").WithTags("Gamification.Profiles").RequireAuthorization();

        profiles.MapGet("/me", GetMyProfileHandler);
        profiles.MapGet("/{userId:guid}", GetProfileByUserIdHandler);
        profiles.MapGet("/me/transactions", GetMyTransactionsHandler);
        profiles.MapGet("/me/earned-awards", GetMyEarnedAwardsHandler);
        profiles.MapGet("/me/progress", GetMyAchievementProgressHandler);
        profiles.MapGet("/{userId:guid}/earned-awards", GetEarnedAwardsByUserIdHandler);
        profiles.MapGet("/{userId:guid}/progress", GetAchievementProgressByUserIdHandler);

        return gamification;
    }

    private static async Task<IResult> GetMyProfileHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetMyProfileQuery(currentUser.TenantId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetProfileByUserIdHandler(
        Guid userId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetProfileByUserIdQuery(currentUser.TenantId, userId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetMyTransactionsHandler(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int limit,
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new GetMyTransactionsQuery(currentUser.TenantId, currentUser.UserId, from, to, limit == 0 ? 50 : limit), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetMyEarnedAwardsHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetMyEarnedAwardsQuery(currentUser.TenantId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetMyAchievementProgressHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetMyAchievementProgressQuery(currentUser.TenantId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetEarnedAwardsByUserIdHandler(
        Guid userId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetMyEarnedAwardsQuery(currentUser.TenantId, userId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetAchievementProgressByUserIdHandler(
        Guid userId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetMyAchievementProgressQuery(currentUser.TenantId, userId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }
}
