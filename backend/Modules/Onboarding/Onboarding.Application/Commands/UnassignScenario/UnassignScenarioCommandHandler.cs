using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

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

        await _assignmentRepository.RemoveAsync(assignment, ct);
        await _assignmentRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
