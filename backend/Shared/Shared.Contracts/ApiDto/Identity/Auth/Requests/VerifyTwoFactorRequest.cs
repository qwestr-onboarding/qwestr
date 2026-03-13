namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;

public record VerifyTwoRequest(
    string TempToken,
    string Code,
    bool IsBackupCode);