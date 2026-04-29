using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Levels.Queries.GetLevelTiers;

public class GetLevelTiersQueryHandler : IRequestHandler<GetLevelTiersQuery, ActionResult<LevelTierListResponse>>
{
    private readonly ILevelTierRepository _levelTierRepository;

    public GetLevelTiersQueryHandler(ILevelTierRepository levelTierRepository)
    {
        _levelTierRepository = levelTierRepository;
    }

    public async Task<ActionResult<LevelTierListResponse>> Handle(GetLevelTiersQuery request, CancellationToken ct)
    {
        var tiers = await _levelTierRepository.GetByTenantAsync(request.TenantId, ct);

        var result = tiers
            .OrderBy(t => t.Order)
            .Select(t => new LevelTierResponse(t.Id, t.Order, t.Title, t.FromLevel, t.ToLevel, t.Color, t.FrameUrl))
            .ToList();

        return ActionResult<LevelTierListResponse>.Success(new LevelTierListResponse(result));
    }
}
