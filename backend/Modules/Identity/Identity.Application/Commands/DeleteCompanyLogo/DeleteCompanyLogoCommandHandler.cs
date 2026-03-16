using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DeleteCompanyLogo;

public class DeleteCompanyLogoCommandHandler : IRequestHandler<DeleteCompanyLogoCommand, ActionResult>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IFileStorageService _fileStorage;

    public DeleteCompanyLogoCommandHandler(ITenantRepository tenantRepository, IFileStorageService fileStorage)
    {
        _tenantRepository = tenantRepository;
        _fileStorage = fileStorage;
    }

    public async Task<ActionResult> Handle(DeleteCompanyLogoCommand request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetWithSettingsAsync(request.TenantId, ct);
        if (tenant is null)
        {
            return ActionResult.Failure("Tenant not found.");
        }

        if (tenant.Settings?.CompanyLogoUrl is null)
        {
            return ActionResult.Failure("Tenant has no logo to delete.");
        }

        await _fileStorage.DeleteAsync(tenant.Settings.CompanyLogoUrl, ct);
        tenant.SetLogo(null);
        await _tenantRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}