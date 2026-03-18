using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetTestAttemptResult;

public class GetTestAttemptResultQueryHandler : IRequestHandler<GetTestAttemptResultQuery, ActionResult<TestResultResponse>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public GetTestAttemptResultQueryHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<TestResultResponse>> Handle(GetTestAttemptResultQuery request, CancellationToken ct)
    {
        var attempt = await _assignmentRepository.GetTestAttemptWithAnswersAsync(request.TestAttemptId, ct);
        if (attempt is null || !attempt.CompletedAt.HasValue)
        {
            return ActionResult<TestResultResponse>.Failure("Test attempt not found or not completed.");
        }

        var stepProgress = await _assignmentRepository.GetStepProgressByIdAsync(attempt.StepProgressId, ct);
        if (stepProgress is null)
        {
            return ActionResult<TestResultResponse>.Failure("Step progress not found.");
        }

        var assignment = await _assignmentRepository.GetByIdAsync(stepProgress.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult<TestResultResponse>.Failure("Test attempt not found.");
        }

        var questionResults = new List<TestQuestionResultResponse>();
        foreach (var answer in attempt.QuestionAnswers)
        {
            var question = await _scenarioRepository.GetQuestionWithOptionsAsync(answer.QuestionId, ct);
            if (question is null)
            {
                continue;
            }

            var correctOptionIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
            questionResults.Add(new TestQuestionResultResponse(
                question.Id,
                question.QuestionText,
                answer.IsCorrect,
                answer.SelectedOptionIds ?? [],
                correctOptionIds));
        }

        var response = new TestResultResponse(
            attempt.Id,
            attempt.Score ?? 0,
            attempt.IsPassed ?? false,
            attempt.AttemptNumber,
            attempt.CompletedAt.Value,
            questionResults);

        return ActionResult<TestResultResponse>.Success(response);
    }
}
