using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetTenantSettings;

public class GetTenantSettingsQueryHandler : IRequestHandler<GetTenantSettingsQuery, ActionResult<TenantSettingsResponse>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantSettingsQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<ActionResult<TenantSettingsResponse>> Handle(GetTenantSettingsQuery request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetWithSettingsAsync(request.TenantId, ct);

        if (tenant is null)
        {
            return ActionResult<TenantSettingsResponse>.Failure("Tenant not found.");
        }

        var settings = tenant.Settings;
        if (settings is null)
        {
            return ActionResult<TenantSettingsResponse>.Failure("Tenant settings not found.");
        }

        return ActionResult<TenantSettingsResponse>.Success(
            new TenantSettingsResponse(tenant.Name, settings.CompanyLogoUrl, settings.Timezone, settings.Language));
    }
}
