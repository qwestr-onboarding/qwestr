using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.AssignScenario;

public class AssignScenarioCommandHandler : IRequestHandler<AssignScenarioCommand, ActionResult<Guid>>
{
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IPublisher _publisher;

    public AssignScenarioCommandHandler(
        IScenarioRepository scenarioRepository,
        IScenarioAssignmentRepository assignmentRepository,
        IPublisher publisher)
    {
        _scenarioRepository = scenarioRepository;
        _assignmentRepository = assignmentRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult<Guid>> Handle(AssignScenarioCommand request, CancellationToken ct)
    {
        var scenario = await _scenarioRepository.GetWithStagesAndStepsAsync(request.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult<Guid>.Failure("Scenario not found.");
        }
        if (!scenario.IsPublished)
        {
            return ActionResult<Guid>.Failure("Cannot assign an unpublished scenario.");
        }

        var alreadyActive = await _assignmentRepository.ExistsActiveAsync(request.TenantId, request.ScenarioId, request.UserId, ct);
        if (alreadyActive)
        {
            return ActionResult<Guid>.Failure("An active assignment for this scenario already exists for the user.");
        }

        var assignment = ScenarioAssignment.Create(request.ScenarioId, request.TenantId, request.UserId, request.AssignedByUserId);

        foreach (var stage in scenario.Stages)
        {
            foreach (var step in stage.Steps)
            {
                var stepProgress = StepProgress.Create(assignment.Id, step.Id);
                await _assignmentRepository.AddStepProgressAsync(stepProgress, ct);
            }
        }

        await _assignmentRepository.AddAsync(assignment, ct);
        await _assignmentRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new OnboardingScenarioAssignedIntegrationEvent(
            request.UserId,
            request.TenantId,
            
            assignment.Id,
            request.ScenarioId,
            scenario.Title,
            request.AssignedByUserId,
            assignment.AssignedAt), ct);

        return ActionResult<Guid>.Success(assignment.Id);
    }
}
