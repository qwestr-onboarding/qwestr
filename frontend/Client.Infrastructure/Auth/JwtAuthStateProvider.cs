using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Infrastructure.Auth;

public class JwtAuthStateProvider(TokenStorageService tokenStorage) : AuthenticationStateProvider
{
    private static readonly AuthenticationState AnonymousState =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = tokenStorage.AccessToken;

        if (string.IsNullOrEmpty(token) || tokenStorage.IsExpired)
            return Task.FromResult(AnonymousState);

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public void NotifyAuthStateChanged()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2) return [];

        var payload = parts[1];
        var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

        byte[] jsonBytes;
        try { jsonBytes = Convert.FromBase64String(padded); }
        catch { return []; }

        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);
        return dict?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()))
               ?? [];
    }
}