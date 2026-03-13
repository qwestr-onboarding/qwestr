using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Modules.Identity.Domain.Events;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;
using DomainRefreshToken = Qwestr.Modules.Identity.Domain.Entities.RefreshToken;

namespace Qwestr.Modules.Identity.Application.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, ActionResult<TokenResponse>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IPublisher _publisher;

    public AcceptInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IPublisher publisher)
    {
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
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

        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user);
        var (refreshTokenValue, refreshExpiresAt) = _tokenService.GenerateRefreshToken();

        var refreshToken = DomainRefreshToken.Create(user.Id, refreshTokenValue, refreshExpiresAt);
        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new UserRegisteredEvent(user.Id, user.TenantId, user.Email, $"{user.FirstName} {user.LastName}"), ct);

        return ActionResult<TokenResponse>.Success(new TokenResponse(accessToken, refreshTokenValue, expiresAt));
    }
}
