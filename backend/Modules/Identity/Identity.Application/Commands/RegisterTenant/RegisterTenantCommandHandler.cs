using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Modules.Identity.Domain.Events;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Contracts.Identity.Events;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Commands.RegisterTenant;

public class RegisterTenantCommandHandler : IRequestHandler<RegisterTenantCommand, ActionResult<TenantRegisteredResponse>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IPublisher _publisher;

    public RegisterTenantCommandHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IPublisher publisher)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _publisher = publisher;
    }

    public async Task<ActionResult<TenantRegisteredResponse>> Handle(RegisterTenantCommand request, CancellationToken ct)
    {
        if (request.Subdomain is not null && await _tenantRepository.ExistsBySubdomainAsync(request.Subdomain, ct))
        {
            return ActionResult<TenantRegisteredResponse>.Failure("Subdomain is already taken.");
        }

        var tenant = Tenant.Create(request.Name, request.Subdomain);
        tenant.Settings = TenantSettings.Create(tenant.Id);

        var passwordHash = _passwordHasher.Hash(request.OwnerPassword);
        var owner = User.Create(tenant.Id, request.OwnerEmail, passwordHash, request.OwnerFirstName, request.OwnerLastName, UserRole.Owner);

        await _tenantRepository.AddAsync(tenant, ct);
        await _userRepository.AddAsync(owner, ct);
        await _tenantRepository.SaveChangesAsync(ct);

        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(owner, tenant.Subdomain);

        await _publisher.Publish(new TenantRegisteredEvent(tenant.Id, tenant.Name), ct);
        await _publisher.Publish(new UserRegisteredEvent(owner.Id, tenant.Id, owner.Email, $"{owner.FirstName} {owner.LastName}"), ct);
        await _publisher.Publish(new UserRegisteredIntegrationEvent(
            owner.Id, owner.TenantId, owner.Email, owner.FirstName, owner.LastName, null, owner.Role), ct);

        return ActionResult<TenantRegisteredResponse>.Success(
            new TenantRegisteredResponse(tenant.Id, accessToken, expiresAt));
    }
}
