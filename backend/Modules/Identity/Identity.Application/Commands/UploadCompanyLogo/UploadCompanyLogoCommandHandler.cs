using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UploadCompanyLogo;

public class UploadCompanyLogoCommandHandler : IRequestHandler<UploadCompanyLogoCommand, ActionResult<string>>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
    private static readonly Dictionary<string, string> ExtensionMap = new()
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private readonly ITenantRepository _tenantRepository;
    private readonly IFileStorageService _fileStorage;

    public UploadCompanyLogoCommandHandler(ITenantRepository tenantRepository, IFileStorageService fileStorage)
    {
        _tenantRepository = tenantRepository;
        _fileStorage = fileStorage;
    }

    public async Task<ActionResult<string>> Handle(UploadCompanyLogoCommand request, CancellationToken ct)
    {
        if (!AllowedContentTypes.Contains(request.ContentType))
        {
            return ActionResult<string>.Failure("Unsupported file type. Allowed: JPEG, PNG, WebP.");
        }

        if (request.FileSize > MaxFileSizeBytes)
        {
            return ActionResult<string>.Failure("File size must not exceed 5 MB.");
        }

        var tenant = await _tenantRepository.GetWithSettingsAsync(request.TenantId, ct);
        if (tenant is null)
        {
            return ActionResult<string>.Failure("Tenant not found.");
        }

        if (tenant.Settings?.CompanyLogoUrl is not null)
        {
            await _fileStorage.DeleteAsync(tenant.Settings.CompanyLogoUrl, ct);
        }

        var extension = ExtensionMap[request.ContentType];
        var fileName = $"{request.TenantId}{extension}";
        var logoUrl = await _fileStorage.UploadAsync("logos", fileName, request.FileStream, ct);

        tenant.SetLogo(logoUrl);
        await _tenantRepository.SaveChangesAsync(ct);

        return ActionResult<string>.Success(logoUrl);
    }
}