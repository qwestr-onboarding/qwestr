using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetTenantBySubdomain;

public class GetTenantBySubdomainQueryHandler
    : IRequestHandler<GetTenantBySubdomainQuery, ActionResult<TenantPublicResponse>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantBySubdomainQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<ActionResult<TenantPublicResponse>> Handle(
        GetTenantBySubdomainQuery request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetWithSettingsBySubdomainAsync(request.Subdomain, ct);

        if (tenant is null || !tenant.IsActive)
        {
            return ActionResult<TenantPublicResponse>.Failure("Tenant not found.");
        }

        return ActionResult<TenantPublicResponse>.Success(
            new TenantPublicResponse(tenant.Name, tenant.Settings?.CompanyLogoUrl));
    }
}
