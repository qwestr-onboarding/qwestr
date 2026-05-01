using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.CompleteInformationStep;

public class CompleteInformationStepCommandHandler : IRequestHandler<CompleteInformationStepCommand, ActionResult>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IPublisher _publisher;

    public CompleteInformationStepCommandHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository,
        IPublisher publisher)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult> Handle(CompleteInformationStepCommand request, CancellationToken ct)
    {
        var stepProgress = await _assignmentRepository.GetStepProgressByIdAsync(request.StepProgressId, ct);
        if (stepProgress is null)
        {
            return ActionResult.Failure("Step progress not found.");
        }

        var assignment = await _assignmentRepository.GetWithProgressAsync(stepProgress.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult.Failure("Step progress not found.");
        }

        if (stepProgress.Status == StepProgressStatus.Completed)
        {
            return ActionResult.Success();
        }

        var step = await _scenarioRepository.GetStepByIdAsync(stepProgress.StepId, ct);
        if (step is null || step.Type != StepType.Information)
        {
            return ActionResult.Failure("This step is not an Information step.");
        }

        stepProgress.Complete();
        await _assignmentRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new OnboardingStepCompletedIntegrationEvent(
            request.UserId,
            request.TenantId,
            assignment.Id,
            stepProgress.StepId,
            assignment.ScenarioId,
            step.Points,
            StepType.Information,
            stepProgress.CompletedAt!.Value), ct);

        var timeSpent = stepProgress.StartedAt.HasValue
            ? stepProgress.CompletedAt!.Value - stepProgress.StartedAt.Value
            : TimeSpan.Zero;

        await _publisher.Publish(new StepCompletedAnalyticsIntegrationEvent(
            request.TenantId,
            request.UserId,
            assignment.Id,
            assignment.ScenarioId,
            step.StageId,
            stepProgress.StepId,
            StepType.Information,
            stepProgress.CompletedAt!.Value,
            timeSpent,
            WasSuccessful: true,
            Score: null,
            MaxScore: null,
            AttemptNumber: 1), ct);

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
                assignment.CompletedAt!.Value), ct);
        }

        return ActionResult.Success();
    }
}
