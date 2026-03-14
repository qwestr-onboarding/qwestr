using System.Net.Http.Headers;
using Client.Infrastructure.Auth;

namespace QwestrWebClient.Endpoints;

public static class BffAuthEndpoints
{
    public static WebApplication MapBffAuth(this WebApplication app)
    {
        var group = app.MapGroup("/bff/auth").AllowAnonymous();

        group.MapPost("/register",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/register",
                    forwardCookies: false, setCookies: true));

        group.MapPost("/login",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/login",
                    forwardCookies: false, setCookies: true));

        group.MapPost("/login/2fa",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/login/2fa",
                    forwardCookies: false, setCookies: true));

        group.MapPost("/refresh",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/refresh",
                    forwardCookies: true, setCookies: true));

        group.MapPost("/logout",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/logout",
                    forwardCookies: true, setCookies: true, forwardAuth: true));

        group.MapPost("/password/forgot",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/password/forgot",
                    forwardCookies: false, setCookies: false));

        group.MapPost("/password/reset",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/auth/password/reset",
                    forwardCookies: false, setCookies: false));

        group.MapPost("/invitations/accept",
            (HttpContext ctx, IHttpClientFactory f) =>
                ProxyAsync(ctx, f, "/api/identity/invitations/accept",
                    forwardCookies: false, setCookies: true));

        return app;
    }

    private static async Task ProxyAsync(
        HttpContext ctx, IHttpClientFactory factory,
        string backendPath, bool forwardCookies, bool setCookies, bool forwardAuth = false)
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

        if (forwardCookies)
        {
            ForwardCookies(ctx, request);
        }
        if (forwardAuth)
        {
            ForwardAuth(ctx, request);
        }
        var response = await client.SendAsync(request);

        if (setCookies)
            await ProxyResponseWithCookies(response, ctx);
        else
            await ProxyResponse(response, ctx);
    }

    private static void ForwardAuth(HttpContext ctx, HttpRequestMessage request)
    {
        var authHeader = ctx.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }
    }
    
    private static void ForwardCookies(HttpContext ctx, HttpRequestMessage request)
    {
        var cookieHeader = ctx.Request.Headers.Cookie.ToString();
        if (!string.IsNullOrEmpty(cookieHeader))
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
    }

    private static async Task ProxyResponseWithCookies(
        HttpResponseMessage backendResponse, HttpContext ctx)
    {
        if (backendResponse.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookie in cookies)
            {
                ctx.Response.Headers.Append("Set-Cookie", cookie);
            }
        }

        await WriteResponse(backendResponse, ctx);
    }

    private static Task ProxyResponse(HttpResponseMessage backendResponse, HttpContext ctx)
        => WriteResponse(backendResponse, ctx);

    private static async Task WriteResponse(HttpResponseMessage backendResponse, HttpContext ctx)
    {
        ctx.Response.StatusCode = (int)backendResponse.StatusCode;
        if (backendResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return;
        }
        ctx.Response.ContentType = "application/json";
        var body = await backendResponse.Content.ReadAsByteArrayAsync();
        await ctx.Response.Body.WriteAsync(body);
    }
}