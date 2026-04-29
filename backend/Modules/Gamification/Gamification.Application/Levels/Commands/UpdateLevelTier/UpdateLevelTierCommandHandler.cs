using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Levels.Commands.UpdateLevelTier;

public class UpdateLevelTierCommandHandler : IRequestHandler<UpdateLevelTierCommand, ActionResult>
{
    private readonly ILevelTierRepository _levelTierRepository;

    public UpdateLevelTierCommandHandler(ILevelTierRepository levelTierRepository)
    {
        _levelTierRepository = levelTierRepository;
    }

    public async Task<ActionResult> Handle(UpdateLevelTierCommand request, CancellationToken ct)
    {
        var tier = await _levelTierRepository.GetByIdAsync(request.Id, ct);
        if (tier == null || tier.TenantId != request.TenantId)
        {
            return ActionResult.Failure("LevelTier.NotFound");
        }

        tier.Update(request.Title, request.FromLevel, request.ToLevel, request.Color, request.FrameUrl);
        await _levelTierRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
