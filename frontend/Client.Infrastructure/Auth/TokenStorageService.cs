using System.Text.Json;

namespace Client.Infrastructure.Auth;

public class TokenStorageService
{
    public string? AccessToken { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string TenantTimezone { get; private set; } = "Europe/Moscow";

    public void SetToken(string token, DateTime expiresAt)
    {
        AccessToken = token;
        ExpiresAt = expiresAt;
        TenantTimezone = ExtractClaim(token, "tenant_timezone") ?? "Europe/Moscow";
    }

    public void Clear()
    {
        AccessToken = null;
        ExpiresAt = null;
        TenantTimezone = "Europe/Moscow";
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;

    private static string? ExtractClaim(string jwt, string claimName)
    {
        try
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2) return null;
            var payload = parts[1];
            var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Convert.FromBase64String(padded));
            return dict?.TryGetValue(claimName, out var el) == true ? el.GetString() : null;
        }
        catch { return null; }
    }
}