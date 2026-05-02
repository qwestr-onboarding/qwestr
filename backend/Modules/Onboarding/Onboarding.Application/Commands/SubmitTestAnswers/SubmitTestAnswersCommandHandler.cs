using System.Text.Json;
using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.SubmitTestAnswers;

public class SubmitTestAnswersCommandHandler : IRequestHandler<SubmitTestAnswersCommand, ActionResult<TestResultResponse>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IPublisher _publisher;

    public SubmitTestAnswersCommandHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository,
        IPublisher publisher)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult<TestResultResponse>> Handle(SubmitTestAnswersCommand request, CancellationToken ct)
    {
        var attempt = await _assignmentRepository.GetTestAttemptByIdAsync(request.TestAttemptId, ct);
        if (attempt is null)
        {
            return ActionResult<TestResultResponse>.Failure("Test attempt not found.");
        }

        if (attempt.CompletedAt != null)
        {
            return ActionResult<TestResultResponse>.Failure("Test attempt already completed.");
        }

        var stepProgress = await _assignmentRepository.GetStepProgressByIdAsync(attempt.StepProgressId, ct);
        if (stepProgress is null)
        {
            return ActionResult<TestResultResponse>.Failure("Step progress not found.");
        }

        var assignment = await _assignmentRepository.GetWithProgressAsync(stepProgress.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult<TestResultResponse>.Failure("Test attempt not found.");
        }

        var step = await _scenarioRepository.GetStepByIdAsync(stepProgress.StepId, ct);
        if (step is null || step.Type != StepType.Test)
        {
            return ActionResult<TestResultResponse>.Failure("This step is not a Test step.");
        }

        var questionResults = new List<TestQuestionResultResponse>();
        var correctCount = 0;
        var totalCount = request.Answers.Count;

        foreach (var answerDto in request.Answers)
        {
            var question = await _scenarioRepository.GetQuestionWithOptionsAsync(answerDto.QuestionId, ct);
            if (question is null)
            {
                continue;
            }

            bool isCorrect;
            List<Guid> correctOptionIds;

            if (question.Type == QuestionType.OpenText)
            {
                isCorrect = !string.IsNullOrWhiteSpace(answerDto.TextAnswer);
                correctOptionIds = [];
            }
            else
            {
                correctOptionIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
                var selected = answerDto.SelectedOptionIds ?? [];
                isCorrect = selected.Count == correctOptionIds.Count && selected.All(id => correctOptionIds.Contains(id));
            }

            if (isCorrect)
            {
                correctCount++;
            }

            var answer = QuestionAnswer.CreateForTest(
                request.TestAttemptId,
                answerDto.QuestionId,
                answerDto.SelectedOptionIds,
                answerDto.TextAnswer,
                isCorrect);

            await _assignmentRepository.AddQuestionAnswerAsync(answer, ct);

            questionResults.Add(new TestQuestionResultResponse(
                question.Id,
                question.QuestionText,
                isCorrect,
                answerDto.SelectedOptionIds ?? [],
                correctOptionIds));
        }

        var score = totalCount > 0 ? (correctCount * 100) / totalCount : 0;

        var passingScore = 70;
        var maxAttempts = 3;
        if (!string.IsNullOrEmpty(step.Settings))
        {
            var settings = JsonSerializer.Deserialize<JsonElement>(step.Settings);
            if (settings.TryGetProperty("passingThreshold", out var passingScoreEl) && passingScoreEl.TryGetInt32(out var parsed))
            {
                passingScore = parsed;
            }
            if (settings.TryGetProperty("maxAttempts", out var maxAttemptsEl) && maxAttemptsEl.TryGetInt32(out var parsedMax))
            {
                maxAttempts = parsedMax;
            }
        }

        var isPassed = score >= passingScore;
        attempt.Complete(score, isPassed);

        if (isPassed)
        {
            stepProgress.Complete();
        }
        else
        {
            stepProgress.Fail();
        }

        await _assignmentRepository.SaveChangesAsync(ct);

        var testTimeSpent = attempt.CompletedAt.HasValue
            ? attempt.CompletedAt.Value - attempt.StartedAt
            : TimeSpan.Zero;

        await _publisher.Publish(new StepCompletedAnalyticsIntegrationEvent(
            request.TenantId,
            request.UserId,
            assignment.Id,
            assignment.ScenarioId,
            step.StageId,
            stepProgress.StepId,
            StepType.Test,
            attempt.CompletedAt!.Value,
            testTimeSpent,
            WasSuccessful: isPassed,
            Score: correctCount,
            MaxScore: totalCount,
            AttemptNumber: attempt.AttemptNumber), ct);

        if (isPassed)
        {
            await _publisher.Publish(new OnboardingStepCompletedIntegrationEvent(
                request.UserId,
                request.TenantId,
                assignment.Id,
                stepProgress.StepId,
                assignment.ScenarioId,
                step.Points,
                StepType.Test,
                stepProgress.CompletedAt!.Value), ct);

            var allCompleted = assignment.StepProgresses.All(sp => sp.Status == StepProgressStatus.Completed);
            if (allCompleted)
            {
                assignment.Complete();
                await _assignmentRepository.SaveChangesAsync(ct);

                var scenario = await _scenarioRepository.GetWithStagesAndStepsAsync(assignment.ScenarioId, ct);
                var totalPoints = scenario?.Stages.SelectMany(s => s.Steps).Sum(s => s.Points) ?? 0;

                await _publisher.Publish(new OnboardingScenarioCompletedIntegrationEvent(
                    request.UserId,
                    request.TenantId,
                    assignment.Id,
                    assignment.ScenarioId,
                    scenario?.Title ?? string.Empty,
                    totalPoints,
                    assignment.CompletedAt!.Value,
                    assignment.AssignedAt), ct);
            }
        }
        else
        {
            var attemptsExhausted = maxAttempts > 0 && attempt.AttemptNumber >= maxAttempts;
            if (attemptsExhausted)
            {
                var allDone = assignment.StepProgresses.All(sp =>
                    sp.Status == StepProgressStatus.Completed || sp.Status == StepProgressStatus.Failed);
                if (allDone)
                {
                    assignment.Complete();
                    await _assignmentRepository.SaveChangesAsync(ct);

                    var scenario = await _scenarioRepository.GetWithStagesAndStepsAsync(assignment.ScenarioId, ct);
                    var totalPoints = scenario?.Stages.SelectMany(s => s.Steps).Sum(s => s.Points) ?? 0;

                    await _publisher.Publish(new OnboardingScenarioCompletedIntegrationEvent(
                        request.UserId,
                        request.TenantId,
                        assignment.Id,
                        assignment.ScenarioId,
                        scenario?.Title ?? string.Empty,
                        totalPoints,
                        assignment.CompletedAt!.Value,
                        assignment.AssignedAt), ct);
                }
            }
        }

        return ActionResult<TestResultResponse>.Success(new TestResultResponse(
            attempt.Id,
            score,
            isPassed,
            attempt.AttemptNumber,
            attempt.CompletedAt!.Value,
            questionResults));
    }
}
