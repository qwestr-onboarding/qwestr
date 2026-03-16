using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.Identity.Events;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ActionResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPublisher _publisher;

    public DeactivateUserCommandHandler(IUserRepository userRepository, IPublisher publisher)
    {
        _userRepository = userRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult> Handle(DeactivateUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.TargetUserId, ct);

        if (user is null || user.TenantId != request.TenantId)
        {
            return ActionResult.Failure("User not found.");
        }

        if (!user.IsActive)
        {
            return ActionResult.Success();
        }

        user.Deactivate();
        await _userRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new UserDeactivatedIntegrationEvent(user.Id, user.TenantId), ct);

        return ActionResult.Success();
    }
}