using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateQuestionOption;

public record UpdateQuestionOptionCommand(
    Guid TenantId,
    Guid OptionId,
    string Text,
    bool IsCorrect) : IRequest<ActionResult>;
