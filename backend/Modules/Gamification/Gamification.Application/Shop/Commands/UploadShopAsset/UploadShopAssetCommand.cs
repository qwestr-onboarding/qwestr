using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.UploadShopAsset;

public record UploadShopAssetCommand(string FileName, Stream Stream) : IRequest<ActionResult<string>>;
