namespace Qwestr.Modules.Identity.Infrastructure.Services;

public class JwtSettings
{
    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessTokenExpiryDays { get; set; } = 2;
}
