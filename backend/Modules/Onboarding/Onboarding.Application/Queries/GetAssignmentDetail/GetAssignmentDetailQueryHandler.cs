using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetAssignmentDetail;

public class GetAssignmentDetailQueryHandler : IRequestHandler<GetAssignmentDetailQuery, ActionResult<AssignmentDetailResponse>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public GetAssignmentDetailQueryHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<AssignmentDetailResponse>> Handle(GetAssignmentDetailQuery request, CancellationToken ct)
    {
        var assignment = await _assignmentRepository.GetWithProgressAsync(request.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId)
        {
            return ActionResult<AssignmentDetailResponse>.Failure("Assignment not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(assignment.ScenarioId, ct);

        var stepProgressSummaries = new List<StepProgressSummaryResponse>();
        foreach (var sp in assignment.StepProgresses)
        {
            var step = await _scenarioRepository.GetStepByIdAsync(sp.StepId, ct);
            stepProgressSummaries.Add(new StepProgressSummaryResponse(
                sp.Id,
                sp.StepId,
                step?.Title ?? string.Empty,
                sp.Status,
                sp.StartedAt,
                sp.CompletedAt));
        }

        var response = new AssignmentDetailResponse(
            assignment.Id,
            assignment.ScenarioId,
            scenario?.Title ?? string.Empty,
            assignment.UserId,
            assignment.AssignedBy,
            assignment.Status,
            assignment.AssignedAt,
            assignment.CompletedAt,
            stepProgressSummaries);

        return ActionResult<AssignmentDetailResponse>.Success(response);
    }
}
