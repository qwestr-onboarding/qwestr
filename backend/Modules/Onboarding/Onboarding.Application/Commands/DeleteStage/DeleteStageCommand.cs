using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteStage;

public record DeleteStageCommand(
    Guid TenantId,
    Guid StageId) : IRequest<ActionResult>;
