using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Onboarding.Application.Commands;
using Qwestr.Modules.Onboarding.Application.Commands.CreateQuestion;
using Qwestr.Modules.Onboarding.Application.Commands.DeleteQuestion;
using Qwestr.Modules.Onboarding.Application.Commands.MoveQuestion;
using Qwestr.Modules.Onboarding.Application.Commands.UpdateQuestion;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    private static IEndpointRouteBuilder MapQuestionsEndpoints(this IEndpointRouteBuilder onboarding)
    {
        var questions = onboarding.MapGroup("questions").WithTags("Onboarding.Questions")
            .RequireAuthorization(HrPolicy);

        questions.MapPost("/", CreateQuestionHandler);
        questions.MapPut("/{id:guid}", UpdateQuestionHandler);
        questions.MapDelete("/{id:guid}", DeleteQuestionHandler);
        questions.MapPost("/{id:guid}/move", MoveQuestionHandler);

        return onboarding;
    }

    private static async Task<IResult> CreateQuestionHandler(
        CreateQuestionRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateQuestionCommand(currentUser.TenantId, body.StepId, body.Type, body.QuestionText), ct);
        return result.IsSuccess
            ? Results.Created($"/api/onboarding/questions/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateQuestionHandler(
        Guid id, UpdateQuestionRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateQuestionCommand(currentUser.TenantId, id, body.Type, body.QuestionText), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeleteQuestionHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteQuestionCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> MoveQuestionHandler(
        Guid id, [FromBody] MoveRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var direction = body.Direction.ToLowerInvariant() == "up" ? MoveDirection.Up : MoveDirection.Down;
        var result = await sender.Send(new MoveQuestionCommand(currentUser.TenantId, id, direction), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
