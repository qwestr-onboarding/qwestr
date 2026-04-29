using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Contracts.Identity.Events;

namespace Qwestr.Modules.Gamification.Application.EventHandlers;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredIntegrationEvent>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly ILevelTierRepository _levelTierRepository;

    public UserRegisteredEventHandler(
        IPlayerProfileRepository profileRepository,
        ILevelTierRepository levelTierRepository)
    {
        _profileRepository = profileRepository;
        _levelTierRepository = levelTierRepository;
    }

    public async Task Handle(UserRegisteredIntegrationEvent notification, CancellationToken ct)
    {
        var existing = await _profileRepository.GetByUserIdAsync(notification.TenantId, notification.UserId, ct);
        if (existing != null)
        {
            return;
        }

        var profile = PlayerProfile.Create(notification.TenantId, notification.UserId);
        await _profileRepository.AddAsync(profile, ct);

        var hasTiers = await _levelTierRepository.HasTiersForTenantAsync(notification.TenantId, ct);
        if (!hasTiers)
        {
            var defaultTiers = CreateDefaultTiers(notification.TenantId);
            await _levelTierRepository.AddRangeAsync(defaultTiers, ct);
        }

        await _profileRepository.SaveChangesAsync(ct);
    }

    private static IEnumerable<LevelTier> CreateDefaultTiers(Guid tenantId)
    {
        var definitions = new[]
        {
            (Order: 1, Title: "Новичок",      From: 1,  To: 10, Color: "#9E9E9E"),
            (Order: 2, Title: "Начинающий",   From: 11, To: 20, Color: "#4CAF50"),
            (Order: 3, Title: "Специалист",   From: 21, To: 30, Color: "#2196F3"),
            (Order: 4, Title: "Эксперт",      From: 31, To: 40, Color: "#9C27B0"),
            (Order: 5, Title: "Мастер",       From: 41, To: 50, Color: "#FFC107"),
        };

        foreach (var d in definitions)
        {
            var result = LevelTier.Create(tenantId, d.Order, d.Title, d.From, d.To, d.Color, null);
            if (result.IsSuccess)
            {
                yield return result.Value!;
            }
        }
    }
}
