using System.Text.Json;

namespace Client.Infrastructure.Auth;

public class TokenStorageService
{
    public string? AccessToken { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    public void SetToken(string token, DateTime expiresAt)
    {
        AccessToken = token;
        ExpiresAt = expiresAt;
    }

    public void Clear()
    {
        AccessToken = null;
        ExpiresAt = null;
    }
    
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
}