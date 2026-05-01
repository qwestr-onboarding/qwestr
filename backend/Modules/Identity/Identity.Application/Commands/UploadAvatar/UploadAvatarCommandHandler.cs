using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UploadAvatar;

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, ActionResult<string>>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
    private static readonly Dictionary<string, string> ExtensionMap = new()
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorage;

    public UploadAvatarCommandHandler(IUserRepository userRepository, IFileStorageService fileStorage)
    {
        _userRepository = userRepository;
        _fileStorage = fileStorage;
    }

    public async Task<ActionResult<string>> Handle(UploadAvatarCommand request, CancellationToken ct)
    {
        if (!AllowedContentTypes.Contains(request.ContentType))
        {
            return ActionResult<string>.Failure("Unsupported file type. Allowed: JPEG, PNG, WebP.");
        }

        if (request.FileSize > MaxFileSizeBytes)
        {
            return ActionResult<string>.Failure("File size must not exceed 5 MB.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
        {
            return ActionResult<string>.Failure("User not found.");
        }

        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            await _fileStorage.DeleteAsync(user.AvatarUrl, ct);
        }

        var extension = ExtensionMap[request.ContentType];
        var fileName = $"{request.UserId}{extension}";
        var avatarUrl = await _fileStorage.UploadAsync("avatars", fileName, request.FileStream, ct);

        user.UpdateAvatar(avatarUrl);
        await _userRepository.SaveChangesAsync(ct);

        return ActionResult<string>.Success(avatarUrl);
    }
}
