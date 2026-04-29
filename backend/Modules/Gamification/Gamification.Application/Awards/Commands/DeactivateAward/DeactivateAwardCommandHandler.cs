using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.DeactivateAward;

public class DeactivateAwardCommandHandler : IRequestHandler<DeactivateAwardCommand, ActionResult>
{
    private readonly IAwardRepository _awardRepository;

    public DeactivateAwardCommandHandler(IAwardRepository awardRepository)
    {
        _awardRepository = awardRepository;
    }

    public async Task<ActionResult> Handle(DeactivateAwardCommand request, CancellationToken ct)
    {
        var award = await _awardRepository.GetByIdAsync(request.Id, ct);
        if (award == null || (award.TenantId != null && award.TenantId != request.TenantId))
        {
            return ActionResult.Failure("Award.NotFound");
        }

        award.Deactivate();
        await _awardRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
