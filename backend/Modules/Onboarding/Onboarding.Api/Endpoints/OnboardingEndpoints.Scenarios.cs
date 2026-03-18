using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Onboarding.Application.Commands.CreateScenario;
using Qwestr.Modules.Onboarding.Application.Commands.DeleteScenario;
using Qwestr.Modules.Onboarding.Application.Commands.DuplicateScenario;
using Qwestr.Modules.Onboarding.Application.Commands.PublishScenario;
using Qwestr.Modules.Onboarding.Application.Commands.UnpublishScenario;
using Qwestr.Modules.Onboarding.Application.Commands.UpdateScenario;
using Qwestr.Modules.Onboarding.Application.Queries.GetScenarioDetail;
using Qwestr.Modules.Onboarding.Application.Queries.GetScenariosList;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    private static IEndpointRouteBuilder MapScenariosEndpoints(this IEndpointRouteBuilder onboarding)
    {
        var scenarios = onboarding.MapGroup("scenarios").WithTags("Onboarding.Scenarios").RequireAuthorization(HrPolicy);

        scenarios.MapGet("/", GetScenariosListHandler);
        scenarios.MapPost("/", CreateScenarioHandler);
        scenarios.MapGet("/{id:guid}", GetScenarioDetailHandler);
        scenarios.MapPut("/{id:guid}", UpdateScenarioHandler);
        scenarios.MapDelete("/{id:guid}", DeleteScenarioHandler);
        scenarios.MapPost("/{id:guid}/publish", PublishScenarioHandler);
        scenarios.MapPost("/{id:guid}/unpublish", UnpublishScenarioHandler);
        scenarios.MapPost("/{id:guid}/duplicate", DuplicateScenarioHandler);

        return onboarding;
    }

    private static async Task<IResult> GetScenariosListHandler(
        [FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? tag,
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetScenariosListQuery(currentUser.TenantId, page, pageSize, tag), ct);
        return result.IsSuccess
            ? Results.Ok(new PagedListResponse<ScenarioListItemResponse>(result.Value!))
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> CreateScenarioHandler(
        CreateScenarioRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateScenarioCommand(currentUser.TenantId, currentUser.UserId, body.Title, body.Description, body.Tags ?? []), ct);
        return result.IsSuccess
            ? Results.Created($"/api/onboarding/scenarios/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetScenarioDetailHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetScenarioDetailQuery(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateScenarioHandler(
        Guid id, UpdateScenarioRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateScenarioCommand(currentUser.TenantId, id, body.Title, body.Description, body.Tags ?? []), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeleteScenarioHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteScenarioCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> PublishScenarioHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new PublishScenarioCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UnpublishScenarioHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new UnpublishScenarioCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DuplicateScenarioHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DuplicateScenarioCommand(currentUser.TenantId, id, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Created($"/api/onboarding/scenarios/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
