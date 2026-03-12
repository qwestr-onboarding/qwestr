using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, ActionResult>
{
    private readonly ITenantRepository _tenantRepository;

    public UpdateTenantCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<ActionResult> Handle(UpdateTenantCommand request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, ct);

        if (tenant is null)
        {
            return ActionResult.Failure("Tenant not found.");
        }

        if (request.Subdomain is not null && request.Subdomain != tenant.Subdomain)
        {
            var subdomainTaken = await _tenantRepository.ExistsBySubdomainAsync(request.Subdomain, request.TenantId, ct);
            if (subdomainTaken)
            {
                return ActionResult.Failure("Subdomain is already taken.");
            }
        }

        tenant.UpdateInfo(request.Name, request.Subdomain);
        await _tenantRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
