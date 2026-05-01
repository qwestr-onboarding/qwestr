using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Modules.Identity.Domain.Events;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Contracts.Identity.Events;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, ActionResult<TokenResponse>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IPublisher _publisher;

    public AcceptInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IPublisher publisher)
    {
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _publisher = publisher;
    }

    public async Task<ActionResult<TokenResponse>> Handle(AcceptInvitationCommand request, CancellationToken ct)
    {
        var invitation = await _invitationRepository.GetByTokenAsync(request.Token, ct);

        if (invitation is null || invitation.Status != InvitationStatus.Pending)
        {
            return ActionResult<TokenResponse>.Failure("Invitation is invalid or has already been used.");
        }

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            return ActionResult<TokenResponse>.Failure("Invitation has expired.");
        }

        var emailTaken = await _userRepository.ExistsByEmailAsync(invitation.TenantId, invitation.Email, ct);
        if (emailTaken)
        {
            return ActionResult<TokenResponse>.Failure("A user with this email already exists.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(invitation.TenantId, invitation.Email, passwordHash, request.FirstName, request.LastName, invitation.Role);

        await _userRepository.AddAsync(user, ct);

        invitation.Accept();
        await _invitationRepository.SaveChangesAsync(ct);

        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, ct);
        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user, tenant?.Subdomain);

        await _publisher.Publish(new UserRegisteredEvent(user.Id, user.TenantId, user.Email, $"{user.FirstName} {user.LastName}"), ct);
        await _publisher.Publish(new UserRegisteredIntegrationEvent(
            user.Id, user.TenantId, user.Email, user.FirstName, user.LastName, null, user.Role), ct);
        await _publisher.Publish(new UserActivatedIntegrationEvent(
            user.TenantId, user.Id, user.Email, user.FirstName, user.LastName, tenant?.Name ?? string.Empty), ct);

        return ActionResult<TokenResponse>.Success(new TokenResponse(accessToken, expiresAt));
    }
}
