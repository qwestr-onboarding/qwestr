using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateQuestionOption;

public record CreateQuestionOptionCommand(
    Guid TenantId,
    Guid QuestionId,
    string Text,
    bool IsCorrect) : IRequest<ActionResult<Guid>>;
