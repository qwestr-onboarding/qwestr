using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Application.Abstractions.Services;
using Qwestr.Modules.Gamification.Application.Awarding;
using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Application.Points;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Enums.Gamification;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Gamification.Application.EventHandlers;

public class OnboardingStepCompletedEventHandler : INotificationHandler<OnboardingStepCompletedIntegrationEvent>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IPointsService _pointsService;
    private readonly IAwardEvaluator _awardEvaluator;
    private readonly ITenantConfigurationProvider _configProvider;

    public OnboardingStepCompletedEventHandler(
        IPlayerProfileRepository profileRepository,
        IPointsService pointsService,
        IAwardEvaluator awardEvaluator,
        ITenantConfigurationProvider configProvider)
    {
        _profileRepository = profileRepository;
        _pointsService = pointsService;
        _awardEvaluator = awardEvaluator;
        _configProvider = configProvider;
    }

    public async Task Handle(OnboardingStepCompletedIntegrationEvent notification, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(notification.TenantId, notification.UserId, ct);
        if (profile == null)
        {
            profile = PlayerProfile.Create(notification.TenantId, notification.UserId);
            await _profileRepository.AddAsync(profile, ct);
            await _profileRepository.SaveChangesAsync(ct);
        }

        var xp = notification.PointsAwarded ?? 0;
        if (xp > 0)
        {
            var rate = await _configProvider.GetCoinsPerXpRateAsync(notification.TenantId, ct);
            var coins = (int)Math.Round(xp * rate);

            await _pointsService.AwardAsync(
                notification.TenantId,
                profile.Id,
                xp,
                coins,
                PointsSourceType.OnboardingStep,
                notification.StepId,
                $"Onboarding step completed",
                ct);
        }

        var stepPayload = new Dictionary<string, object>
        {
            ["stepId"] = notification.StepId,
            ["scenarioId"] = notification.ScenarioId,
            ["stepType"] = notification.StepType.ToString()
        };

        var newEarned = await _awardEvaluator.EvaluateAsync(
            new EvaluationContext(notification.TenantId, profile.Id, RuleTrigger.StepCompleted, stepPayload), ct);

        if (notification.StepType == StepType.Test)
        {
            var testPayload = new Dictionary<string, object>
            {
                ["stepId"] = notification.StepId,
                ["score"] = xp
            };

            newEarned = (await _awardEvaluator.EvaluateAsync(
                new EvaluationContext(notification.TenantId, profile.Id, RuleTrigger.TestPassed, testPayload), ct))
                .Concat(newEarned).ToList();
        }

        var iteration = 0;
        while (newEarned.Count > 0 && iteration < 3)
        {
            iteration++;
            var refreshedProfile = await _profileRepository.GetByIdAsync(profile.Id, ct);
            if (refreshedProfile == null)
            {
                break;
            }

            var xpPayload = new Dictionary<string, object> { ["totalXp"] = (object)refreshedProfile.TotalXp };
            var chainEarned = await _awardEvaluator.EvaluateAsync(
                new EvaluationContext(notification.TenantId, profile.Id, RuleTrigger.PointsAccumulated, xpPayload), ct);
            newEarned = chainEarned;
        }
    }
}
