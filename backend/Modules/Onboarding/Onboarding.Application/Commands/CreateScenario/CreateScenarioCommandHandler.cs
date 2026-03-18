using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateScenario;

public class CreateScenarioCommandHandler : IRequestHandler<CreateScenarioCommand, ActionResult<Guid>>
{
    private readonly IScenarioRepository _scenarioRepository;

    public CreateScenarioCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateScenarioCommand request, CancellationToken ct)
    {
        var scenario = Scenario.Create(request.TenantId, request.Title, request.Description, request.Tags);

        await _scenarioRepository.AddAsync(scenario, ct);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(scenario.Id);
    }
}
