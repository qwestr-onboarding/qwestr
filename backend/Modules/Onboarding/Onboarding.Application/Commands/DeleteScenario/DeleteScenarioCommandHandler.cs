using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteScenario;

public class DeleteScenarioCommandHandler : IRequestHandler<DeleteScenarioCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IPublisher _publisher;

    public DeleteScenarioCommandHandler(IScenarioRepository scenarioRepository, IPublisher publisher)
    {
        _scenarioRepository = scenarioRepository;
        _publisher = publisher;
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

        _scenarioRepository.Remove(scenario);
        await _scenarioRepository.SaveChangesAsync(ct);

        await _publisher.Publish(
            new ScenarioDeletedIntegrationEvent(request.TenantId, request.ScenarioId), ct);

        return ActionResult.Success();
    }
}
