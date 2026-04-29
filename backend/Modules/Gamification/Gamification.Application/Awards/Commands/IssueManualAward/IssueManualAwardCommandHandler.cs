using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Application.Points;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Modules.Gamification.Domain.Events;
using Qwestr.Shared.Contracts.Gamification.Events;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.IssueManualAward;

public class IssueManualAwardCommandHandler : IRequestHandler<IssueManualAwardCommand, ActionResult>
{
    private readonly IAwardRepository _awardRepository;
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IPointsService _pointsService;
    private readonly IPublisher _publisher;

    public IssueManualAwardCommandHandler(
        IAwardRepository awardRepository,
        IPlayerProfileRepository profileRepository,
        IPointsService pointsService,
        IPublisher publisher)
    {
        _awardRepository = awardRepository;
        _profileRepository = profileRepository;
        _pointsService = pointsService;
        _publisher = publisher;
    }

    public async Task<ActionResult> Handle(IssueManualAwardCommand request, CancellationToken ct)
    {
        var award = await _awardRepository.GetByIdAsync(request.AwardId, ct);
        if (award == null || award.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Award.NotFound");
        }
        if (award.IssuanceType != AwardIssuanceType.Manual)
        {
            return ActionResult.Failure("Award.NotManual");
        }
        if (!award.IsActive)
        {
            return ActionResult.Failure("Award.Inactive");
        }

        var profile = await _profileRepository.GetByIdAsync(request.ProfileId, ct);
        if (profile == null || profile.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Profile.NotFound");
        }

        var alreadyEarned = await _awardRepository.IsAwardAlreadyEarnedAsync(request.ProfileId, request.AwardId, ct);
        if (alreadyEarned)
        {
            return ActionResult.Failure("Award.AlreadyIssued");
        }

        var earnedAward = EarnedAward.CreateManual(request.TenantId, request.ProfileId, request.AwardId, request.IssuedByUserId);
        await _awardRepository.AddEarnedAwardAsync(earnedAward, ct);
        await _awardRepository.SaveChangesAsync(ct);

        if (award.XpBonus > 0 || award.CoinsBonus > 0)
        {
            await _pointsService.AwardAsync(
                request.TenantId, request.ProfileId,
                award.XpBonus, award.CoinsBonus,
                PointsSourceType.AwardBonus, earnedAward.Id,
                $"Award: {award.Code}", ct);
        }

        await _publisher.Publish(new AwardEarnedEvent(request.TenantId, request.ProfileId, profile.UserId, request.AwardId, AwardIssuanceType.Manual), ct);
        await _publisher.Publish(new AwardEarnedIntegrationEvent(request.TenantId, profile.UserId, request.AwardId, award.Code, earnedAward.EarnedAt), ct);

        return ActionResult.Success();
    }
}
