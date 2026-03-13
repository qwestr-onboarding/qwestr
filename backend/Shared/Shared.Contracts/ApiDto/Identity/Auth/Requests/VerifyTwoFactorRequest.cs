namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;

public record VerifyTwoFactorRequest(
    string TempToken,
    string Code,
    bool IsBackupCode);