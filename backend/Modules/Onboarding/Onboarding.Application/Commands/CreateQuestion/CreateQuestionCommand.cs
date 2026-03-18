using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateQuestion;

public record CreateQuestionCommand(
    Guid TenantId,
    Guid StepId,
    QuestionType Type,
    string QuestionText) : IRequest<ActionResult<Guid>>;
