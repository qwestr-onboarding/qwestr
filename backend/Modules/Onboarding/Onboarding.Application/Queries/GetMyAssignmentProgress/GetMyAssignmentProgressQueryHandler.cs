using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetMyAssignmentProgress;

public class GetMyAssignmentProgressQueryHandler : IRequestHandler<GetMyAssignmentProgressQuery, ActionResult<MyAssignmentProgressResponse>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public GetMyAssignmentProgressQueryHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<MyAssignmentProgressResponse>> Handle(GetMyAssignmentProgressQuery request, CancellationToken ct)
    {
        var assignment = await _assignmentRepository.GetWithProgressAsync(request.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult<MyAssignmentProgressResponse>.Failure("Assignment not found.");
        }

        var scenario = await _scenarioRepository.GetWithStagesAndStepsAsync(assignment.ScenarioId, ct);
        if (scenario is null)
        {
            return ActionResult<MyAssignmentProgressResponse>.Failure("Scenario not found.");
        }

        var stepProgressMap = assignment.StepProgresses.ToDictionary(sp => sp.StepId);

        var stages = scenario.Stages.OrderBy(s => s.Order).Select(stage => new StageProgressResponse(
            stage.Id,
            stage.Title,
            stage.Order,
            stage.Steps.OrderBy(st => st.Order).Select(step =>
            {
                stepProgressMap.TryGetValue(step.Id, out var sp);
                return new StepProgressStatusResponse(
                    step.Id,
                    sp?.Id ?? Guid.Empty,
                    step.Title,
                    step.Type,
                    step.Order,
                    step.Points,
                    sp?.Status ?? Qwestr.Shared.Kernel.Enums.Onboarding.StepProgressStatus.NotStarted);
            }).ToList()
        )).ToList();

        var response = new MyAssignmentProgressResponse(
            assignment.Id,
            assignment.ScenarioId,
            scenario.Title,
            assignment.Status,
            assignment.AssignedAt,
            assignment.CompletedAt,
            stages);

        return ActionResult<MyAssignmentProgressResponse>.Success(response);
    }
}
