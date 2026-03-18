using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.UnassignScenario;

public class UnassignScenarioCommandHandler : IRequestHandler<UnassignScenarioCommand, ActionResult>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;

    public UnassignScenarioCommandHandler(IScenarioAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<ActionResult> Handle(UnassignScenarioCommand request, CancellationToken ct)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Assignment not found.");
        }

        if (assignment.Status != AssignmentStatus.NotStarted)
        {
            return ActionResult.Failure("Can only unassign scenarios that have not been started.");
        }

        // Mark as completed with a tombstone is not the pattern here — remove the entity
        // Since we don't have a Remove on assignment, we handle it via the DbContext directly
        // This requires the infrastructure layer to expose this. For now delegate to repository.
        await _assignmentRepository.RemoveAsync(assignment, ct);
        await _assignmentRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
