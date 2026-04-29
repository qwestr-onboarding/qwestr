using MediatR;
using Qwestr.Modules.Gamification.Application.Awarding;
using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Domain.Events;
using Qwestr.Shared.Contracts.Gamification.Events;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.EventHandlers;

public class PlayerLevelUpEventHandler : INotificationHandler<PlayerLevelUpEvent>
{
    private readonly IAwardEvaluator _awardEvaluator;
    private readonly IPublisher _publisher;

    public PlayerLevelUpEventHandler(IAwardEvaluator awardEvaluator, IPublisher publisher)
    {
        _awardEvaluator = awardEvaluator;
        _publisher = publisher;
    }

    public async Task Handle(PlayerLevelUpEvent notification, CancellationToken ct)
    {
        var payload = new Dictionary<string, object> { ["newLevel"] = (object)notification.NewLevel };

        await _awardEvaluator.EvaluateAsync(
            new EvaluationContext(notification.TenantId, notification.ProfileId, RuleTrigger.LevelReached, payload), ct);

        await _publisher.Publish(new PlayerLevelUpIntegrationEvent(
            notification.TenantId,
            notification.UserId,
            notification.OldLevel,
            notification.NewLevel,
            DateTime.UtcNow), ct);
    }
}
