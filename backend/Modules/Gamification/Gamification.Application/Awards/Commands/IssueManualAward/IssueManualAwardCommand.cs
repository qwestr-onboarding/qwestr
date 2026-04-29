using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.IssueManualAward;

public record IssueManualAwardCommand(Guid TenantId, Guid AwardId, Guid ProfileId, Guid IssuedByUserId) : IRequest<ActionResult>;
