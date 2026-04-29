using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.CreateManualAward;

public class CreateManualAwardCommandHandler : IRequestHandler<CreateManualAwardCommand, ActionResult<Guid>>
{
    private readonly IAwardRepository _awardRepository;

    public CreateManualAwardCommandHandler(IAwardRepository awardRepository)
    {
        _awardRepository = awardRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateManualAwardCommand request, CancellationToken ct)
    {
        var result = Award.CreateManual(request.TenantId, request.Code, request.Title, request.Description, request.IconUrl, request.XpBonus, request.CoinsBonus);
        if (result.IsFailure)
        {
            return ActionResult<Guid>.Failure(result.Error);
        }

        await _awardRepository.AddAsync(result.Value!, ct);
        await _awardRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(result.Value!.Id);
    }
}
