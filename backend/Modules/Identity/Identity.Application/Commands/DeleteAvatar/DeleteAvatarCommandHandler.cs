using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DeleteAvatar;

public class DeleteAvatarCommandHandler : IRequestHandler<DeleteAvatarCommand, ActionResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorage;

    public DeleteAvatarCommandHandler(IUserRepository userRepository, IFileStorageService fileStorage)
    {
        _userRepository = userRepository;
        _fileStorage = fileStorage;
    }

    public async Task<ActionResult> Handle(DeleteAvatarCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
        {
            return ActionResult.Failure("User not found.");
        }

        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            await _fileStorage.DeleteAsync(user.AvatarUrl, ct);
        }

        user.UpdateAvatar(null);
        await _userRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
