using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.MoveQuestion;

public record MoveQuestionCommand(
    Guid TenantId,
    Guid QuestionId,
    MoveDirection Direction) : IRequest<ActionResult>;
