using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Application.Abstractions.Services;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Modules.Gamification.Domain.ValueObjects;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ActionResult<PlayerProfileResponse>>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly ILevelTierRepository _levelTierRepository;
    private readonly IShopItemRepository _shopItemRepository;
    private readonly ILevelCalculator _levelCalculator;

    public GetMyProfileQueryHandler(
        IPlayerProfileRepository profileRepository,
        ILevelTierRepository levelTierRepository,
        IShopItemRepository shopItemRepository,
        ILevelCalculator levelCalculator)
    {
        _profileRepository = profileRepository;
        _levelTierRepository = levelTierRepository;
        _shopItemRepository = shopItemRepository;
        _levelCalculator = levelCalculator;
    }

    public async Task<ActionResult<PlayerProfileResponse>> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            profile = PlayerProfile.Create(request.TenantId, request.UserId);
            await _profileRepository.AddAsync(profile, ct);
            await _profileRepository.SaveChangesAsync(ct);
        }

        var tier = await _levelTierRepository.GetForLevelAsync(request.TenantId, profile.CurrentLevel, ct);
        var xpToNext = _levelCalculator.XpRequiredForNextLevel(profile.TotalXp);
        var equipment = await BuildEquipmentResponseAsync(profile.Equipment, ct);

        var response = new PlayerProfileResponse(
            profile.Id,
            profile.UserId,
            profile.TenantId,
            profile.TotalXp,
            profile.AvailableCoins,
            profile.CurrentLevel,
            xpToNext,
            tier?.Title,
            tier?.Color,
            tier?.FrameUrl,
            profile.Status,
            equipment,
            profile.LastActivityAt,
            profile.CreatedAt);

        return ActionResult<PlayerProfileResponse>.Success(response);
    }

    private async Task<EquipmentResponse> BuildEquipmentResponseAsync(Equipment equipment, CancellationToken ct)
    {
        var ids = new[] {
            equipment.ProfileColorItemId,
            equipment.AvatarFrameItemId,
            equipment.BackgroundItemId,
            equipment.PetItemId,
            equipment.ClassItemId,
            equipment.WeaponItemId
        }.Where(id => id.HasValue).Select(id => id!.Value);

        var items = await _shopItemRepository.GetByIdsAsync(ids, ct);
        var dict = items.ToDictionary(i => i.Id);

        EquippedItemResponse? Map(Guid? id) => id.HasValue && dict.TryGetValue(id.Value, out var item)
            ? new EquippedItemResponse(item.Id, item.Name, item.IconUrl, item.AssetData)
            : null;

        return new EquipmentResponse(
            Map(equipment.ProfileColorItemId),
            Map(equipment.AvatarFrameItemId),
            Map(equipment.BackgroundItemId),
            Map(equipment.PetItemId),
            Map(equipment.ClassItemId),
            Map(equipment.WeaponItemId));
    }
}
