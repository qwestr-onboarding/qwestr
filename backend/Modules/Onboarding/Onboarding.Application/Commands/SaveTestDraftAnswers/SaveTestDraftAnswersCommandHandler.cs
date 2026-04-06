using System.Text.Json;
using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.SaveTestDraftAnswers;

public class SaveTestDraftAnswersCommandHandler : IRequestHandler<SaveTestDraftAnswersCommand, ActionResult>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;

    public SaveTestDraftAnswersCommandHandler(IScenarioAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<ActionResult> Handle(SaveTestDraftAnswersCommand request, CancellationToken ct)
    {
        var attempt = await _assignmentRepository.GetTestAttemptByIdAsync(request.TestAttemptId, ct);
        if (attempt is null || attempt.CompletedAt.HasValue)
            return ActionResult.Failure("Test attempt not found or already completed.");

        var stepProgress = await _assignmentRepository.GetStepProgressByIdAsync(attempt.StepProgressId, ct);
        if (stepProgress is null)
            return ActionResult.Failure("Step progress not found.");

        var assignment = await _assignmentRepository.GetByIdAsync(stepProgress.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
            return ActionResult.Failure("Test attempt not found.");

        var draftJson = JsonSerializer.Serialize(request.Answers, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await _assignmentRepository.SaveTestAttemptDraftAsync(request.TestAttemptId, draftJson, ct);

        return ActionResult.Success();
    }
}
