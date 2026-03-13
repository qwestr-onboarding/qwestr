using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetTenant;

public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, ActionResult<TenantInfoResponse>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<ActionResult<TenantInfoResponse>> Handle(GetTenantQuery request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, ct);

        if (tenant is null)
        {
            return ActionResult<TenantInfoResponse>.Failure("Tenant not found.");
        }

        return ActionResult<TenantInfoResponse>.Success(
            new TenantInfoResponse(tenant.Id, tenant.Name, tenant.Subdomain, tenant.Plan, tenant.IsActive, tenant.CreatedAt));
    }
}
