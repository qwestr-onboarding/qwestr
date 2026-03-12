using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UpdateTenantSettings;

public class UpdateTenantSettingsCommandHandler : IRequestHandler<UpdateTenantSettingsCommand, ActionResult>
{
    private readonly ITenantRepository _tenantRepository;

    public UpdateTenantSettingsCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<ActionResult> Handle(UpdateTenantSettingsCommand request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetWithSettingsAsync(request.TenantId, ct);

        if (tenant is null)
        {
            return ActionResult.Failure("Tenant not found.");
        }

        tenant.UpdateSettings(request.Timezone, request.Language, request.CompanyLogoUrl);
        await _tenantRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
