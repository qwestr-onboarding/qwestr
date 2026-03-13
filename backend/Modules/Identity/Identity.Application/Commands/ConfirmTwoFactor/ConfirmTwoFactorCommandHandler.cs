using System.Security.Cryptography;
using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Modules.Identity.Domain.Events;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ConfirmTwoFactor;

public class ConfirmTwoFactorCommandHandler : IRequestHandler<ConfirmTwoFactorCommand, ActionResult<BackupCodesResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITotpService _totpService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPublisher _publisher;

    public ConfirmTwoFactorCommandHandler(
        IUserRepository userRepository,
        ITotpService totpService,
        IPasswordHasher passwordHasher,
        IPublisher publisher)
    {
        _userRepository = userRepository;
        _totpService = totpService;
        _passwordHasher = passwordHasher;
        _publisher = publisher;
    }

    public async Task<ActionResult<BackupCodesResponse>> Handle(ConfirmTwoFactorCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);

        if (user is null)
        {
            return ActionResult<BackupCodesResponse>.Failure("Пользователь не найден.");
        }

        if (user.TwoFactorEnabled)
        {
            return ActionResult<BackupCodesResponse>.Failure("Двухфакторная аутентификация уже включена.");
        }

        if (string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            return ActionResult<BackupCodesResponse>.Failure("Двухфакторная настройка не была запущена. Сначала выполните настройку.");
        }

        if (!_totpService.VerifyCode(user.TwoFactorSecret, request.Code))
        {
            return ActionResult<BackupCodesResponse>.Failure("Неверный проверочный код.");
        }

        user.ConfirmTwoFactor();

        var plainCodes = GenerateBackupCodes(8);
        var backupCodes = plainCodes
            .Select(code => UserBackupCode.Create(user.Id, _passwordHasher.Hash(code)))
            .ToList();

        await _userRepository.AddBackupCodesAsync(backupCodes, ct);
        await _userRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new TwoFactorEnabledEvent(user.Id, user.Email), ct);

        return ActionResult<BackupCodesResponse>.Success(new BackupCodesResponse(plainCodes));
    }

    private static List<string> GenerateBackupCodes(int count)
    {
        var codes = new List<string>(count);
        for (var i = 0; i < count; i++)
        {
            var bytes = new byte[5];
            RandomNumberGenerator.Fill(bytes);
            codes.Add(Convert.ToHexString(bytes).ToLower());
        }
        return codes;
    }
}
