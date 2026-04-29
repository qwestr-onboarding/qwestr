using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Levels.Commands.CreateLevelTier;

public class CreateLevelTierCommandHandler : IRequestHandler<CreateLevelTierCommand, ActionResult<Guid>>
{
    private readonly ILevelTierRepository _levelTierRepository;

    public CreateLevelTierCommandHandler(ILevelTierRepository levelTierRepository)
    {
        _levelTierRepository = levelTierRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateLevelTierCommand request, CancellationToken ct)
    {
        var result = LevelTier.Create(request.TenantId, request.Order, request.Title, request.FromLevel, request.ToLevel, request.Color, request.FrameUrl);
        if (result.IsFailure)
        {
            return ActionResult<Guid>.Failure(result.Error);
        }

        await _levelTierRepository.AddAsync(result.Value!, ct);
        await _levelTierRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(result.Value!.Id);
    }
}
