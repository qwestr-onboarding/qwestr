using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Onboarding.Application.Commands.AnswerQuestionStep;
using Qwestr.Modules.Onboarding.Application.Commands.CompleteChecklistStep;
using Qwestr.Modules.Onboarding.Application.Commands.CompleteInformationStep;
using Qwestr.Modules.Onboarding.Application.Commands.CompleteTaskStep;
using Qwestr.Modules.Onboarding.Application.Commands.StartStep;
using Qwestr.Modules.Onboarding.Application.Commands.StartTestAttempt;
using Qwestr.Modules.Onboarding.Application.Commands.SaveTestDraftAnswers;
using Qwestr.Modules.Onboarding.Application.Commands.SubmitTestAnswers;
using Qwestr.Modules.Onboarding.Application.Commands.UpdateChecklistProgress;
using Qwestr.Modules.Onboarding.Application.Queries.GetMyAssignmentProgress;
using Qwestr.Modules.Onboarding.Application.Queries.GetMyAssignments;
using Qwestr.Modules.Onboarding.Application.Queries.GetStepForEmployee;
using Qwestr.Modules.Onboarding.Application.Queries.GetTestAttemptResult;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    private static IEndpointRouteBuilder MapProgressEndpoints(this IEndpointRouteBuilder onboarding)
    {
        var my = onboarding.MapGroup("my").WithTags("Onboarding.Employee").RequireAuthorization();

        // Мои назначения
        my.MapGet("/assignments", GetMyAssignmentsHandler);
        my.MapGet("/assignments/{assignmentId:guid}/progress", GetMyAssignmentProgressHandler);
        my.MapGet("/assignments/{assignmentId:guid}/steps/{stepId:guid}", GetStepForEmployeeHandler);
        my.MapPost("/assignments/{assignmentId:guid}/steps/{stepId:guid}/start", StartStepHandler);

        // Прохождение шагов
        my.MapPost("/progress/{progressId:guid}/complete-information", CompleteInformationStepHandler);
        my.MapPost("/progress/{progressId:guid}/complete-task", CompleteTaskStepHandler);
        my.MapPut("/progress/{progressId:guid}/checklist", UpdateChecklistProgressHandler);
        my.MapPost("/progress/{progressId:guid}/complete-checklist", CompleteChecklistStepHandler);
        my.MapPost("/progress/{progressId:guid}/answer", AnswerQuestionStepHandler);
        my.MapPost("/progress/{progressId:guid}/test/start", StartTestAttemptHandler);

        // Тест
        my.MapPut("/test/{attemptId:guid}/draft", SaveTestDraftAnswersHandler);
        my.MapPost("/test/{attemptId:guid}/submit", SubmitTestAnswersHandler);
        my.MapGet("/test/{attemptId:guid}/result", GetTestAttemptResultHandler);

        return onboarding;
    }

    private static async Task<IResult> GetMyAssignmentsHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetMyAssignmentsQuery(currentUser.TenantId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetMyAssignmentProgressHandler(
        Guid assignmentId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new GetMyAssignmentProgressQuery(currentUser.TenantId, currentUser.UserId, assignmentId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetStepForEmployeeHandler(
        Guid assignmentId, Guid stepId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new GetStepForEmployeeQuery(currentUser.TenantId, currentUser.UserId, assignmentId, stepId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> StartStepHandler(
        Guid assignmentId, Guid stepId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new StartStepCommand(currentUser.TenantId, currentUser.UserId, assignmentId, stepId), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> CompleteInformationStepHandler(
        Guid progressId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CompleteInformationStepCommand(currentUser.TenantId, currentUser.UserId, progressId), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> CompleteTaskStepHandler(
        Guid progressId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CompleteTaskStepCommand(currentUser.TenantId, currentUser.UserId, progressId), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateChecklistProgressHandler(
        Guid progressId, UpdateChecklistProgressRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateChecklistProgressCommand(currentUser.TenantId, currentUser.UserId, progressId, body.CheckedIndices), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> CompleteChecklistStepHandler(
        Guid progressId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CompleteChecklistStepCommand(currentUser.TenantId, currentUser.UserId, progressId), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> AnswerQuestionStepHandler(
        Guid progressId, AnswerQuestionStepRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new AnswerQuestionStepCommand(currentUser.TenantId, currentUser.UserId, progressId, body.QuestionId,
                body.SelectedOptionIds, body.TextAnswer), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> StartTestAttemptHandler(
        Guid progressId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new StartTestAttemptCommand(currentUser.TenantId, currentUser.UserId, progressId), ct);
        return result.IsSuccess
            ? Results.Ok(new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> SaveTestDraftAnswersHandler(
        Guid attemptId, SaveTestDraftAnswersRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var answers = body.Answers.Select(a =>
            new DraftAnswerDto(a.QuestionId, a.SelectedOptionIds, a.TextAnswer)).ToList();

        var result = await sender.Send(
            new SaveTestDraftAnswersCommand(currentUser.TenantId, currentUser.UserId, attemptId, answers), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> SubmitTestAnswersHandler(
        Guid attemptId, SubmitTestAnswersRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var answers = body.Answers.Select(a =>
            new TestQuestionAnswerDto(a.QuestionId, a.SelectedOptionIds, a.TextAnswer)).ToList();

        var result = await sender.Send(
            new SubmitTestAnswersCommand(currentUser.TenantId, currentUser.UserId, attemptId, answers), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetTestAttemptResultHandler(
        Guid attemptId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new GetTestAttemptResultQuery(currentUser.TenantId, currentUser.UserId, attemptId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }
}
