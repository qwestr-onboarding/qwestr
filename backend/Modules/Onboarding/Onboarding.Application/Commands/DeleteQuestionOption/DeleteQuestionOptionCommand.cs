using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteQuestionOption;

public record DeleteQuestionOptionCommand(
    Guid TenantId,
    Guid OptionId) : IRequest<ActionResult>;
