using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ActionResult>
{
    public Task<ActionResult> Handle(LogoutCommand request, CancellationToken ct)
        => Task.FromResult(ActionResult.Success());
}
