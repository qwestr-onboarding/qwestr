using System.Text.Json;
using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateChecklistProgress;

public class UpdateChecklistProgressCommandHandler : IRequestHandler<UpdateChecklistProgressCommand, ActionResult>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public UpdateChecklistProgressCommandHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(UpdateChecklistProgressCommand request, CancellationToken ct)
    {
        var stepProgress = await _assignmentRepository.GetStepProgressByIdAsync(request.StepProgressId, ct);
        if (stepProgress is null)
        {
            return ActionResult.Failure("Step progress not found.");
        }

        var assignment = await _assignmentRepository.GetByIdAsync(stepProgress.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult.Failure("Step progress not found.");
        }

        var step = await _scenarioRepository.GetStepByIdAsync(stepProgress.StepId, ct);
        if (step is null || step.Type != StepType.Checklist)
        {
            return ActionResult.Failure("This step is not a Checklist step.");
        }

        var json = JsonSerializer.Serialize(request.CheckedIndices);
        stepProgress.UpdateCheckedItems(json);

        await _assignmentRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
