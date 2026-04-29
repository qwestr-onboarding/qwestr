using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Levels.Commands.DeleteLevelTier;

public class DeleteLevelTierCommandHandler : IRequestHandler<DeleteLevelTierCommand, ActionResult>
{
    private readonly ILevelTierRepository _levelTierRepository;

    public DeleteLevelTierCommandHandler(ILevelTierRepository levelTierRepository)
    {
        _levelTierRepository = levelTierRepository;
    }

    public async Task<ActionResult> Handle(DeleteLevelTierCommand request, CancellationToken ct)
    {
        var tier = await _levelTierRepository.GetByIdAsync(request.Id, ct);
        if (tier == null || tier.TenantId != request.TenantId)
        {
            return ActionResult.Failure("LevelTier.NotFound");
        }

        _levelTierRepository.Remove(tier);
        await _levelTierRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
