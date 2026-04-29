using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.UpdateAward;

public class UpdateAwardCommandHandler : IRequestHandler<UpdateAwardCommand, ActionResult>
{
    private readonly IAwardRepository _awardRepository;

    public UpdateAwardCommandHandler(IAwardRepository awardRepository)
    {
        _awardRepository = awardRepository;
    }

    public async Task<ActionResult> Handle(UpdateAwardCommand request, CancellationToken ct)
    {
        var award = await _awardRepository.GetByIdAsync(request.Id, ct);
        if (award == null || (award.TenantId != null && award.TenantId != request.TenantId))
        {
            return ActionResult.Failure("Award.NotFound");
        }

        award.Update(request.Title, request.Description, request.IconUrl);
        await _awardRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
