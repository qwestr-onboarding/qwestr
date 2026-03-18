using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteQuestion;

public record DeleteQuestionCommand(
    Guid TenantId,
    Guid QuestionId) : IRequest<ActionResult>;
