namespace Qwestr.Modules.Identity.Application.Abstractions.Services;

public interface ITotpService
{
    string GenerateSecret();
    string GenerateQrCodeUri(string email, string secret);
    bool VerifyCode(string secret, string code);
}
