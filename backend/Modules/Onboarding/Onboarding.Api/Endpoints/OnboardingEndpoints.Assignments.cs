using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Onboarding.Application.Commands.AssignScenario;
using Qwestr.Modules.Onboarding.Application.Commands.UnassignScenario;
using Qwestr.Modules.Onboarding.Application.Queries.GetAssignmentDetail;
using Qwestr.Modules.Onboarding.Application.Queries.GetAssignmentsList;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    private static IEndpointRouteBuilder MapAssignmentsEndpoints(this IEndpointRouteBuilder onboarding)
    {
        var assignments = onboarding.MapGroup("assignments").WithTags("Onboarding.Assignments").RequireAuthorization(HrPolicy);

        assignments.MapGet("/", GetAssignmentsListHandler);
        assignments.MapPost("/", AssignScenarioHandler);
        assignments.MapGet("/{id:guid}", GetAssignmentDetailHandler);
        assignments.MapDelete("/{id:guid}", UnassignScenarioHandler);

        return onboarding;
    }

    private static async Task<IResult> GetAssignmentsListHandler(
        [FromQuery] int page, [FromQuery] int pageSize,
        [FromQuery] Guid? scenarioId, [FromQuery] Guid? userId,
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new GetAssignmentsListQuery(currentUser.TenantId, page, pageSize, scenarioId, userId), ct);
        return result.IsSuccess
            ? Results.Ok(new PagedListResponse<AssignmentListItemResponse>(result.Value!))
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> AssignScenarioHandler(
        AssignScenarioRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new AssignScenarioCommand(currentUser.TenantId, body.ScenarioId, body.UserId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Created($"/api/onboarding/assignments/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetAssignmentDetailHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetAssignmentDetailQuery(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UnassignScenarioHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new UnassignScenarioCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
