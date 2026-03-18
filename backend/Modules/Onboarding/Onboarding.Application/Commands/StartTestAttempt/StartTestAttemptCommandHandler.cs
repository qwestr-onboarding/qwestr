using System.Text.Json;
using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.StartTestAttempt;

public class StartTestAttemptCommandHandler : IRequestHandler<StartTestAttemptCommand, ActionResult<Guid>>
{
    private readonly IScenarioAssignmentRepository _assignmentRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public StartTestAttemptCommandHandler(
        IScenarioAssignmentRepository assignmentRepository,
        IScenarioRepository scenarioRepository)
    {
        _assignmentRepository = assignmentRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<Guid>> Handle(StartTestAttemptCommand request, CancellationToken ct)
    {
        var stepProgress = await _assignmentRepository.GetStepProgressByIdAsync(request.StepProgressId, ct);
        if (stepProgress is null)
        {
            return ActionResult<Guid>.Failure("Step progress not found.");
        }

        var assignment = await _assignmentRepository.GetByIdAsync(stepProgress.AssignmentId, ct);
        if (assignment is null || assignment.TenantId != request.TenantId || assignment.UserId != request.UserId)
        {
            return ActionResult<Guid>.Failure("Step progress not found.");
        }

        var step = await _scenarioRepository.GetStepByIdAsync(stepProgress.StepId, ct);
        if (step is null || step.Type != StepType.Test)
        {
            return ActionResult<Guid>.Failure("This step is not a Test step.");
        }

        if (!string.IsNullOrEmpty(step.Settings))
        {
            var settings = JsonSerializer.Deserialize<JsonElement>(step.Settings);
            if (settings.TryGetProperty("maxAttempts", out var maxAttemptsEl) && maxAttemptsEl.TryGetInt32(out var maxAttempts))
            {
                var usedAttempts = await _assignmentRepository.CountTestAttemptsAsync(request.StepProgressId, ct);
                if (usedAttempts >= maxAttempts)
                {
                    return ActionResult<Guid>.Failure("Maximum number of test attempts reached.");
                }
            }
        }

        var lastAttemptNumber = await _assignmentRepository.GetLastAttemptNumberAsync(request.StepProgressId, ct);
        var attempt = TestAttempt.Create(request.StepProgressId, lastAttemptNumber + 1);

        stepProgress.Start();

        await _assignmentRepository.AddTestAttemptAsync(attempt, ct);
        await _assignmentRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(attempt.Id);
    }
}
