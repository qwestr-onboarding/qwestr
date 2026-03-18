using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.MoveQuestionOption;

public record MoveQuestionOptionCommand(
    Guid TenantId,
    Guid OptionId,
    MoveDirection Direction) : IRequest<ActionResult>;
