using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.DeleteAward;

public class DeleteAwardCommandHandler : IRequestHandler<DeleteAwardCommand, ActionResult>
{
    private readonly IAwardRepository _awardRepository;

    public DeleteAwardCommandHandler(IAwardRepository awardRepository)
    {
        _awardRepository = awardRepository;
    }

    public async Task<ActionResult> Handle(DeleteAwardCommand request, CancellationToken ct)
    {
        var award = await _awardRepository.GetByIdAsync(request.Id, ct);
        if (award is null || (award.TenantId != null && award.TenantId != request.TenantId))
        {
            return ActionResult.Failure("Award.NotFound");
        }

        await _awardRepository.RemoveAsync(award, ct);
        await _awardRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
