using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Onboarding.Application.Commands;
using Qwestr.Modules.Onboarding.Application.Commands.CreateStep;
using Qwestr.Modules.Onboarding.Application.Commands.DeleteStep;
using Qwestr.Modules.Onboarding.Application.Commands.MoveStep;
using Qwestr.Modules.Onboarding.Application.Commands.UpdateStep;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    private static IEndpointRouteBuilder MapStepsEndpoints(this IEndpointRouteBuilder onboarding)
    {
        var steps = onboarding.MapGroup("steps").WithTags("Onboarding.Steps")
            .RequireAuthorization(HrPolicy);

        steps.MapPost("/", CreateStepHandler);
        steps.MapPut("/{id:guid}", UpdateStepHandler);
        steps.MapDelete("/{id:guid}", DeleteStepHandler);
        steps.MapPost("/{id:guid}/move", MoveStepHandler);

        return onboarding;
    }

    private static async Task<IResult> CreateStepHandler(
        CreateStepRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateStepCommand(currentUser.TenantId, body.StageId, body.Title, body.Type, body.Points, body.Content, body.Settings), ct);
        return result.IsSuccess
            ? Results.Created($"/api/onboarding/steps/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateStepHandler(
        Guid id, UpdateStepRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateStepCommand(currentUser.TenantId, id, body.Title, body.Type, body.Points, body.Content, body.Settings), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeleteStepHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteStepCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> MoveStepHandler(
        Guid id, [FromBody] MoveRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var direction = body.Direction.ToLowerInvariant() == "up" ? MoveDirection.Up : MoveDirection.Down;
        var result = await sender.Send(new MoveStepCommand(currentUser.TenantId, id, direction), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
