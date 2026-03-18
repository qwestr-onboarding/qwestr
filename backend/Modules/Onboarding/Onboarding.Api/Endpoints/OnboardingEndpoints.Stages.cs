using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Onboarding.Application.Commands;
using Qwestr.Modules.Onboarding.Application.Commands.CreateStage;
using Qwestr.Modules.Onboarding.Application.Commands.DeleteStage;
using Qwestr.Modules.Onboarding.Application.Commands.MoveStage;
using Qwestr.Modules.Onboarding.Application.Commands.UpdateStage;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    private static IEndpointRouteBuilder MapStagesEndpoints(this IEndpointRouteBuilder onboarding)
    {
        var stages = onboarding.MapGroup("stages").WithTags("Onboarding.Stages")
            .RequireAuthorization(HrPolicy);

        stages.MapPost("/", CreateStageHandler);
        stages.MapPut("/{id:guid}", UpdateStageHandler);
        stages.MapDelete("/{id:guid}", DeleteStageHandler);
        stages.MapPost("/{id:guid}/move", MoveStageHandler);

        return onboarding;
    }

    private static async Task<IResult> CreateStageHandler(
        CreateStageRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateStageCommand(currentUser.TenantId, body.ScenarioId, body.Title, body.Description), ct);
        return result.IsSuccess
            ? Results.Created($"/api/onboarding/scenarios/{body.ScenarioId}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateStageHandler(
        Guid id, UpdateStageRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateStageCommand(currentUser.TenantId, id, body.Title, body.Description), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeleteStageHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteStageCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> MoveStageHandler(
        Guid id, [FromBody] MoveRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var direction = body.Direction.ToLowerInvariant() == "up" ? MoveDirection.Up : MoveDirection.Down;
        var result = await sender.Send(new MoveStageCommand(currentUser.TenantId, id, direction), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
