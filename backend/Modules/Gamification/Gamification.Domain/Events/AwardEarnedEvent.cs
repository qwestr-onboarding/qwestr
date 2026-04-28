using MediatR;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Domain.Events;

public sealed record AwardEarnedEvent(Guid TenantId, Guid ProfileId, Guid UserId, Guid AwardId, AwardIssuanceType IssuanceType) : INotification;