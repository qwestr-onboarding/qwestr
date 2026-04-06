using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetStepForEmployee;

public class GetStepForEmployeeQueryHandler : IRequestHandler<GetStepForEmployeeQuery, ActionResult<StepContentResponse>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public GetStepForEmployeeQueryHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<StepContentResponse>> Handle(GetStepForEmployeeQuery request, CancellationToken ct)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult<StepContentResponse>.Failure("Assignment not found.");
        }

        var stepProgress = await _assignmentRepository.GetStepProgressAsync(request.AssignmentId, request.StepId, ct);
        if (stepProgress is null)
        {
            return ActionResult<StepContentResponse>.Failure("Step not found in this assignment.");
        }

        var step = await _scenarioRepository.GetStepWithQuestionsAsync(request.StepId, ct);
        if (step is null)
        {
            return ActionResult<StepContentResponse>.Failure("Step not found.");
        }

        // Strip IsCorrect from options — employee must not see correct answers
        var questions = step.Questions.OrderBy(q => q.Order).Select(q => new QuestionContentResponse(
            q.Id,
            q.Type,
            q.QuestionText,
            q.Order,
            q.Options.OrderBy(o => o.Order).Select(o => new EmployeeOptionResponse(
                o.Id,
                o.Text,
                o.Order)).ToList()
        )).ToList();

        Guid? currentTestAttemptId = null;
        string? currentTestDraftAnswersJson = null;
        if (step.Type == StepType.Test && stepProgress.Status == StepProgressStatus.InProgress)
        {
            currentTestAttemptId = await _assignmentRepository.GetLatestIncompleteAttemptIdAsync(stepProgress.Id, ct);
            if (currentTestAttemptId.HasValue)
                currentTestDraftAnswersJson = await _assignmentRepository.GetTestAttemptDraftAsync(currentTestAttemptId.Value, ct);
        }

        var response = new StepContentResponse(
            step.Id,
            stepProgress.Id,
            step.Title,
            step.Type,
            step.Points,
            step.Content,
            step.Settings,
            stepProgress.Status,
            questions,
            currentTestAttemptId,
            currentTestDraftAnswersJson);

        return ActionResult<StepContentResponse>.Success(response);
    }
}
