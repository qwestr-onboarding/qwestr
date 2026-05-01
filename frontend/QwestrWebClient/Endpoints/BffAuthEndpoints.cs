using System.Text.Json;

namespace QwestrWebClient.Endpoints;

public static class BffAuthEndpoints
{
    private const string CookieName = "qwestr_at";

    public static WebApplication MapBffAuth(this WebApplication app)
    {
        var group = app.MapGroup("/bff/auth").AllowAnonymous();

        group.MapPost("/register",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/register", forwardAuth: false, setCookie: true));

        group.MapPost("/login",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/login", forwardAuth: false, setCookie: true));

        group.MapPost("/login/2fa",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/login/2fa", forwardAuth: false, setCookie: true));

        group.MapPost("/logout",
            (HttpContext ctx, IHttpClientFactory f) =>
                LogoutAsync(ctx, f));

        group.MapPost("/password/forgot",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/password/forgot", forwardAuth: false, setCookie: false));

        group.MapPost("/password/reset",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/password/reset", forwardAuth: false, setCookie: false));

        group.MapPost("/invitations/accept",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/invitations/accept", forwardAuth: false, setCookie: true));

        group.MapPost("/clear-token", (HttpContext ctx) =>
        {
            ctx.Response.Cookies.Delete(CookieName);
            return Results.Ok();
        });

        return app;
    }

    private static async Task LogoutAsync(HttpContext ctx, IHttpClientFactory factory)
    {
        var authHeader = ctx.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader))
        {
            try
            {
                using var client = factory.CreateClient("Backend");
                var req = new HttpRequestMessage(HttpMethod.Post, "/api/identity/auth/logout");
                req.Headers.TryAddWithoutValidation("Authorization", authHeader);
                await client.SendAsync(req);
            }
            catch { }
        }

        ctx.Response.Cookies.Delete(CookieName);
        ctx.Response.StatusCode = StatusCodes.Status200OK;
    }

    private static async Task ProxyAsync(
        HttpContext ctx, IHttpClientFactory factory,
        string backendPath, bool forwardAuth, bool setCookie)
    {
        using var client = factory.CreateClient("Backend");

        using var ms = new MemoryStream();
        await ctx.Request.Body.CopyToAsync(ms);
        var bodyBytes = ms.ToArray();

        var request = new HttpRequestMessage(HttpMethod.Post, backendPath);
        if (bodyBytes.Length > 0)
        {
            request.Content = new ByteArrayContent(bodyBytes);
            request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");
        }

        if (forwardAuth)
        {
            var authHeader = ctx.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(authHeader))
                request.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }

        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsByteArrayAsync();

        if (setCookie && response.IsSuccessStatusCode && body.Length > 0)
            TrySetAuthCookie(ctx, body);

        ctx.Response.StatusCode = (int)response.StatusCode;
        if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Body.WriteAsync(body);
        }
    }

    private static void TrySetAuthCookie(HttpContext ctx, byte[] bodyBytes)
    {
        try
        {
            using var doc = JsonDocument.Parse(bodyBytes);
            var root = doc.RootElement;

            if (root.TryGetProperty("requiresTwoFactor", out var tf) && tf.GetBoolean())
                return;

            if (!root.TryGetProperty("accessToken", out var tokenEl)) return;
            var token = tokenEl.GetString();
            if (string.IsNullOrEmpty(token)) return;

            if (!root.TryGetProperty("expiresAt", out var expiresEl)) return;
            if (!DateTime.TryParse(expiresEl.GetString(), null,
                    System.Globalization.DateTimeStyles.RoundtripKind, out var expiresAt)) return;

            ctx.Response.Cookies.Append(CookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = ctx.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAt,
                Path = "/"
            });
        }
        catch { }
    }
}
