using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateQuestion;

public record UpdateQuestionCommand(
    Guid TenantId,
    Guid QuestionId,
    QuestionType Type,
    string QuestionText) : IRequest<ActionResult>;
