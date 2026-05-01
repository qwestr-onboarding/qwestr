using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Services;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.UploadShopAsset;

public class UploadShopAssetCommandHandler : IRequestHandler<UploadShopAssetCommand, ActionResult<string>>
{
    private readonly IFileStorageService _storage;

    public UploadShopAssetCommandHandler(IFileStorageService storage)
    {
        _storage = storage;
    }

    public async Task<ActionResult<string>> Handle(UploadShopAssetCommand request, CancellationToken ct)
    {
        var ext = Path.GetExtension(request.FileName);
        var safeFileName = $"{Guid.NewGuid()}{ext}";
        var url = await _storage.UploadAsync("shop-assets", safeFileName, request.Stream, ct);
        return ActionResult<string>.Success(url);
    }
}
