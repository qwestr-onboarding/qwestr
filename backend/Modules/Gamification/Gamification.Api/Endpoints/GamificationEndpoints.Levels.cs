using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Gamification.Application.Levels.Commands.CreateLevelTier;
using Qwestr.Modules.Gamification.Application.Levels.Commands.DeleteLevelTier;
using Qwestr.Modules.Gamification.Application.Levels.Commands.UpdateLevelTier;
using Qwestr.Modules.Gamification.Application.Levels.Queries.GetLevelTiers;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Gamification.Api.Endpoints;

public static partial class GamificationEndpoints
{
    private static IEndpointRouteBuilder MapLevelTiersEndpoints(this IEndpointRouteBuilder gamification)
    {
        var tiers = gamification.MapGroup("level-tiers").WithTags("Gamification.Levels").RequireAuthorization(HrPolicy);

        tiers.MapGet("/", GetLevelTiersHandler);
        tiers.MapPost("/", CreateLevelTierHandler);
        tiers.MapPut("/{id:guid}", UpdateLevelTierHandler);
        tiers.MapDelete("/{id:guid}", DeleteLevelTierHandler);

        return gamification;
    }

    private static async Task<IResult> GetLevelTiersHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetLevelTiersQuery(currentUser.TenantId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> CreateLevelTierHandler(
        CreateLevelTierRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateLevelTierCommand(currentUser.TenantId, body.Order, body.Title, body.FromLevel, body.ToLevel, body.Color, body.FrameUrl), ct);
        return result.IsSuccess
            ? Results.Created($"/api/gamification/level-tiers/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateLevelTierHandler(
        Guid id, UpdateLevelTierRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateLevelTierCommand(currentUser.TenantId, id, body.Title, body.FromLevel, body.ToLevel, body.Color, body.FrameUrl), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeleteLevelTierHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteLevelTierCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
