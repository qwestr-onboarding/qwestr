using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.AnswerQuestionStep;

public class AnswerQuestionStepCommandHandler : IRequestHandler<AnswerQuestionStepCommand, ActionResult<AnswerResultResponse>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IPublisher _publisher;

    public AnswerQuestionStepCommandHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository,
        IPublisher publisher)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult<AnswerResultResponse>> Handle(AnswerQuestionStepCommand request, CancellationToken ct)
    {
        var stepProgress = await _assignmentRepository.GetStepProgressByIdAsync(request.StepProgressId, ct);
        if (stepProgress is null)
        {
            return ActionResult<AnswerResultResponse>.Failure("Step progress not found.");
        }

        var assignment = await _assignmentRepository.GetWithProgressAsync(stepProgress.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult<AnswerResultResponse>.Failure("Step progress not found.");
        }

        var question = await _scenarioRepository.GetQuestionWithOptionsAsync(request.QuestionId, ct);
        if (question is null || question.StepId != stepProgress.StepId)
        {
            return ActionResult<AnswerResultResponse>.Failure("Question not found for this step.");
        }

        var step = await _scenarioRepository.GetStepByIdAsync(stepProgress.StepId, ct);
        if (step is null || step.Type != StepType.Question)
        {
            return ActionResult<AnswerResultResponse>.Failure("This step is not a Question step.");
        }

        bool isCorrect;
        List<Guid> correctOptionIds;

        if (question.Type == QuestionType.OpenText)
        {
            isCorrect = true;
            correctOptionIds = [];
        }
        else
        {
            correctOptionIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
            var selected = request.SelectedOptionIds ?? [];
            isCorrect = selected.Count == correctOptionIds.Count && selected.All(id => correctOptionIds.Contains(id));
        }

        var answer = QuestionAnswer.CreateForQuestion(
            request.StepProgressId,
            request.QuestionId,
            request.SelectedOptionIds,
            request.TextAnswer,
            isCorrect);

        await _assignmentRepository.AddQuestionAnswerAsync(answer, ct);

        stepProgress.Complete();
        await _assignmentRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new OnboardingStepCompletedIntegrationEvent(
            request.UserId,
            request.TenantId,
            assignment.Id,
            stepProgress.StepId,
            assignment.ScenarioId,
            step.Points,
            StepType.Question,
            stepProgress.CompletedAt!.Value), ct);

        var allCompleted = assignment.StepProgresses.All(sp => sp.Status == StepProgressStatus.Completed);
        if (allCompleted)
        {
            assignment.Complete();
            await _assignmentRepository.SaveChangesAsync(ct);

            await _publisher.Publish(new OnboardingScenarioCompletedIntegrationEvent(
                request.UserId,
                request.TenantId,
                assignment.Id,
                assignment.ScenarioId,
                assignment.CompletedAt!.Value), ct);
        }

        return ActionResult<AnswerResultResponse>.Success(new AnswerResultResponse(isCorrect, correctOptionIds));
    }
}
