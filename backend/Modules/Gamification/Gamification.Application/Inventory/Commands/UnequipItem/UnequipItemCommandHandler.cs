using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Inventory.Commands.UnequipItem;

public class UnequipItemCommandHandler : IRequestHandler<UnequipItemCommand, ActionResult>
{
    private readonly IPlayerProfileRepository _profileRepository;

    public UnequipItemCommandHandler(IPlayerProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<ActionResult> Handle(UnequipItemCommand request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            return ActionResult.Failure("Profile.NotFound");
        }

        profile.Unequip(request.Slot);
        await _profileRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
