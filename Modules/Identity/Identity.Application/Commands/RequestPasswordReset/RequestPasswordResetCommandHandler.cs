using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Modules.Identity.Domain.Events;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.RequestPasswordReset;

public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, ActionResult>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IPublisher _publisher;

    public RequestPasswordResetCommandHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IPublisher publisher)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult> Handle(RequestPasswordResetCommand request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetBySubdomainAsync(request.Subdomain, ct);
        if (tenant is null || !tenant.IsActive)
        {
            return ActionResult.Success();
        }

        var user = await _userRepository.GetByEmailAsync(tenant.Id, request.Email, ct);
        if (user is null || !user.IsActive)
        {
            return ActionResult.Success();
        }

        await _passwordResetTokenRepository.InvalidateAllByUserIdAsync(user.Id, ct);

        var token = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.AddHours(2);
        var resetToken = PasswordResetToken.Create(user.Id, token, expiresAt);

        await _passwordResetTokenRepository.AddAsync(resetToken, ct);
        await _passwordResetTokenRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new PasswordResetRequestedEvent(user.Id, user.Email, token, expiresAt), ct);

        return ActionResult.Success();
    }
}
