using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.CreateManualAward;

public record CreateManualAwardCommand(
    Guid TenantId,
    string Code,
    string Title,
    string Description,
    string IconUrl,
    int XpBonus,
    int CoinsBonus) : IRequest<ActionResult<Guid>>;
