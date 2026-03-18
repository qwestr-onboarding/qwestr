using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetMyAssignments;

public class GetMyAssignmentsQueryHandler : IRequestHandler<GetMyAssignmentsQuery, ActionResult<List<MyAssignmentSummaryResponse>>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public GetMyAssignmentsQueryHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<List<MyAssignmentSummaryResponse>>> Handle(GetMyAssignmentsQuery request, CancellationToken ct)
    {
        var assignments = await _assignmentRepository.GetByUserAsync(request.TenantId, request.UserId, ct);

        var result = new List<MyAssignmentSummaryResponse>();
        foreach (var assignment in assignments)
        {
            var scenario = await _scenarioRepository.GetByIdAsync(assignment.ScenarioId, ct);
            var withProgress = await _assignmentRepository.GetWithProgressAsync(assignment.Id, ct);
            var completedSteps = withProgress?.StepProgresses.Count(sp => sp.Status == StepProgressStatus.Completed) ?? 0;
            var totalSteps = withProgress?.StepProgresses.Count ?? 0;

            result.Add(new MyAssignmentSummaryResponse(
                assignment.Id,
                assignment.ScenarioId,
                scenario?.Title ?? string.Empty,
                scenario?.Description,
                assignment.Status,
                assignment.AssignedAt,
                assignment.CompletedAt,
                completedSteps,
                totalSteps));
        }

        return ActionResult<List<MyAssignmentSummaryResponse>>.Success(result);
    }
}
