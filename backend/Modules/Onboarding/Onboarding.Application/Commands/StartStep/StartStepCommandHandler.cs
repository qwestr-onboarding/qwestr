using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.StartStep;

public class StartStepCommandHandler : IRequestHandler<StartStepCommand, ActionResult>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;

    public StartStepCommandHandler(IScenarioAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<ActionResult> Handle(StartStepCommand request, CancellationToken ct)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult.Failure("Assignment not found.");
        }

        var stepProgress = await _assignmentRepository.GetStepProgressAsync(request.AssignmentId, request.StepId, ct);
        if (stepProgress is null)
        {
            return ActionResult.Failure("Step not found in this assignment.");
        }

        assignment.Start();
        stepProgress.Start();

        await _assignmentRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
