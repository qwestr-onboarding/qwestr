using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.PublishScenario;

public class PublishScenarioCommandHandler : IRequestHandler<PublishScenarioCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IPublisher _publisher;

    public PublishScenarioCommandHandler(IScenarioRepository scenarioRepository, IPublisher publisher)
    {
        _scenarioRepository = scenarioRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult> Handle(PublishScenarioCommand request, CancellationToken ct)
    {
        var scenario = await _scenarioRepository.GetWithStagesAndStepsAsync(request.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Scenario not found.");
        }

        scenario.Publish();
        await _scenarioRepository.SaveChangesAsync(ct);

        var stages = scenario.Stages
            .OrderBy(s => s.Order)
            .Select(s => new ScenarioStageMetadataDto(
                s.Id,
                s.Title,
                s.Order,
                s.Steps.OrderBy(st => st.Order)
                    .Select(st => new ScenarioStepMetadataDto(st.Id, st.Title, st.Order))
                    .ToList()))
            .ToList();

        await _publisher.Publish(new ScenarioMetadataChangedIntegrationEvent(
            request.TenantId,
            scenario.Id,
            scenario.Title,
            stages), ct);

        return ActionResult.Success();
    }
}
