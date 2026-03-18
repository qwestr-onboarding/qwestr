using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteScenario;

public class DeleteScenarioCommandHandler : IRequestHandler<DeleteScenarioCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public DeleteScenarioCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(DeleteScenarioCommand request, CancellationToken ct)
    {
        var scenario = await _scenarioRepository.GetByIdAsync(request.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Scenario not found.");
        }

        var hasActive = await _scenarioRepository.HasActiveAssignmentsAsync(request.ScenarioId, ct);
        if (hasActive)
        {
            return ActionResult.Failure("Cannot delete scenario with active assignments.");
        }

        var fullScenario = await _scenarioRepository.GetWithFullTreeAsync(request.ScenarioId, ct);
        if (fullScenario is not null)
        {
            foreach (var stage in fullScenario.Stages)
            {
                foreach (var step in stage.Steps)
                {
                    foreach (var question in step.Questions)
                    {
                        foreach (var option in question.Options)
                        {
                            _scenarioRepository.RemoveOption(option);
                        }
                        _scenarioRepository.RemoveQuestion(question);
                    }
                    _scenarioRepository.RemoveStep(step);
                }
                _scenarioRepository.RemoveStage(stage);
            }
        }

        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
