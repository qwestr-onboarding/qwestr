using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Gamification.Application.Awards.Commands.CreateAutomaticAward;
using Qwestr.Modules.Gamification.Application.Awards.Commands.CreateManualAward;
using Qwestr.Modules.Gamification.Application.Awards.Commands.DeactivateAward;
using Qwestr.Modules.Gamification.Application.Awards.Commands.IssueManualAward;
using Qwestr.Modules.Gamification.Application.Awards.Commands.UpdateAward;
using Qwestr.Modules.Gamification.Application.Awards.Queries.GetAwards;
using Qwestr.Modules.Gamification.Application.Awards.Queries.GetAwardsForManagement;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Api.Endpoints;

public static partial class GamificationEndpoints
{
    private static IEndpointRouteBuilder MapAwardsEndpoints(this IEndpointRouteBuilder gamification)
    {
        var awards = gamification.MapGroup("awards").WithTags("Gamification.Awards");

        awards.MapGet("/", GetAwardsHandler).RequireAuthorization();
        awards.MapGet("/manage", GetAwardsForManagementHandler).RequireAuthorization(HrPolicy);
        awards.MapPost("/automatic", CreateAutomaticAwardHandler).RequireAuthorization(HrPolicy);
        awards.MapPost("/manual", CreateManualAwardHandler).RequireAuthorization(HrPolicy);
        awards.MapPut("/{id:guid}", UpdateAwardHandler).RequireAuthorization(HrPolicy);
        awards.MapPost("/{id:guid}/deactivate", DeactivateAwardHandler).RequireAuthorization(HrPolicy);
        awards.MapPost("/{id:guid}/issue", IssueManualAwardHandler).RequireAuthorization(HrPolicy);

        return gamification;
    }

    private static async Task<IResult> GetAwardsHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetAwardsQuery(currentUser.TenantId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetAwardsForManagementHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetAwardsForManagementQuery(currentUser.TenantId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> CreateAutomaticAwardHandler(
        CreateAutomaticAwardRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        if (!Enum.TryParse<RuleTrigger>(body.Trigger, true, out var trigger))
        {
            return Results.BadRequest(new ErrorResponse($"Unknown trigger: {body.Trigger}"));
        }

        if (!Enum.TryParse<RuleCondition>(body.Condition, true, out var condition))
        {
            return Results.BadRequest(new ErrorResponse($"Unknown condition: {body.Condition}"));
        }

        var result = await sender.Send(
            new CreateAutomaticAwardCommand(
                currentUser.TenantId, body.Code, body.Title, body.Description, body.IconUrl,
                trigger, condition, body.TargetValue, body.FilterParams, body.XpBonus, body.CoinsBonus), ct);
        return result.IsSuccess
            ? Results.Created($"/api/gamification/awards/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> CreateManualAwardHandler(
        CreateManualAwardRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateManualAwardCommand(currentUser.TenantId, body.Code, body.Title, body.Description, body.IconUrl, body.XpBonus, body.CoinsBonus), ct);
        return result.IsSuccess
            ? Results.Created($"/api/gamification/awards/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateAwardHandler(
        Guid id, UpdateAwardRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateAwardCommand(currentUser.TenantId, id, body.Title, body.Description, body.IconUrl), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeactivateAwardHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeactivateAwardCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> IssueManualAwardHandler(
        Guid id, IssueManualAwardRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new IssueManualAwardCommand(currentUser.TenantId, id, body.ProfileId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
