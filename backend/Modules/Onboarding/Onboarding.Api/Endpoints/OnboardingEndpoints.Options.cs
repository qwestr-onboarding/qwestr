using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Onboarding.Application.Commands;
using Qwestr.Modules.Onboarding.Application.Commands.CreateQuestionOption;
using Qwestr.Modules.Onboarding.Application.Commands.DeleteQuestionOption;
using Qwestr.Modules.Onboarding.Application.Commands.MoveQuestionOption;
using Qwestr.Modules.Onboarding.Application.Commands.UpdateQuestionOption;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    private static IEndpointRouteBuilder MapOptionsEndpoints(this IEndpointRouteBuilder onboarding)
    {
        var options = onboarding.MapGroup("options").WithTags("Onboarding.Options")
            .RequireAuthorization(HrPolicy);

        options.MapPost("/", CreateOptionHandler);
        options.MapPut("/{id:guid}", UpdateOptionHandler);
        options.MapDelete("/{id:guid}", DeleteOptionHandler);
        options.MapPost("/{id:guid}/move", MoveOptionHandler);

        return onboarding;
    }

    private static async Task<IResult> CreateOptionHandler(
        CreateQuestionOptionRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateQuestionOptionCommand(currentUser.TenantId, body.QuestionId, body.Text, body.IsCorrect), ct);
        return result.IsSuccess
            ? Results.Created($"/api/onboarding/options/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateOptionHandler(
        Guid id, UpdateQuestionOptionRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateQuestionOptionCommand(currentUser.TenantId, id, body.Text, body.IsCorrect), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeleteOptionHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteQuestionOptionCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> MoveOptionHandler(
        Guid id, [FromBody] MoveRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var direction = body.Direction.ToLowerInvariant() == "up" ? MoveDirection.Up : MoveDirection.Down;
        var result = await sender.Send(new MoveQuestionOptionCommand(currentUser.TenantId, id, direction), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
