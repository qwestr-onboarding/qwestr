using OtpNet;
using Qwestr.Modules.Identity.Application.Abstractions.Services;

namespace Qwestr.Modules.Identity.Infrastructure.Services;

public class TotpService : ITotpService
{
    public string GenerateSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string email, string secret)
        => $"otpauth://totp/Qwestr:{Uri.EscapeDataString(email)}?secret={secret}&issuer=Qwestr";

    public bool VerifyCode(string secret, string code)
    {
        var key = Base32Encoding.ToBytes(secret);
        var totp = new Totp(key);
        return totp.VerifyTotp(DateTime.UtcNow, code, out _, new VerificationWindow(1, 1));
    }
}
