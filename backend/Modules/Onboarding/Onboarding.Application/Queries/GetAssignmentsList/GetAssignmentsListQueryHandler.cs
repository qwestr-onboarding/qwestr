using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetAssignmentsList;

public class GetAssignmentsListQueryHandler : IRequestHandler<GetAssignmentsListQuery, ActionResult<PagedList<AssignmentListItemResponse>>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public GetAssignmentsListQueryHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<PagedList<AssignmentListItemResponse>>> Handle(GetAssignmentsListQuery request, CancellationToken ct)
    {
        var assignments = await _assignmentRepository.GetByTenantAsync(request.TenantId, request.Page, request.PageSize, request.ScenarioId, request.UserId, ct);
        var total = await _assignmentRepository.CountByTenantAsync(request.TenantId, request.ScenarioId, request.UserId, ct);

        var items = new List<AssignmentListItemResponse>();
        foreach (var assignment in assignments)
        {
            var scenario = await _scenarioRepository.GetByIdAsync(assignment.ScenarioId, ct);
            var withProgress = await _assignmentRepository.GetWithProgressAsync(assignment.Id, ct);
            var completedSteps = withProgress?.StepProgresses.Count(sp => sp.Status == StepProgressStatus.Completed) ?? 0;
            var totalSteps = withProgress?.StepProgresses.Count ?? 0;

            items.Add(new AssignmentListItemResponse(
                assignment.Id,
                assignment.ScenarioId,
                scenario?.Title ?? string.Empty,
                assignment.UserId,
                assignment.Status,
                assignment.AssignedAt,
                assignment.CompletedAt,
                completedSteps,
                totalSteps));
        }

        return ActionResult<PagedList<AssignmentListItemResponse>>.Success(
            new PagedList<AssignmentListItemResponse>(items, request.Page, request.PageSize, total));
    }
}
