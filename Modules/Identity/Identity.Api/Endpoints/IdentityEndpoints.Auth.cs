using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Identity.Application.Commands.Login;
using Qwestr.Modules.Identity.Application.Commands.Logout;
using Qwestr.Modules.Identity.Application.Commands.RefreshToken;
using Qwestr.Modules.Identity.Application.Commands.RegisterTenant;
using Qwestr.Modules.Identity.Application.Commands.RequestPasswordReset;
using Qwestr.Modules.Identity.Application.Commands.ResetPassword;
using Qwestr.Modules.Identity.Application.Commands.VerifyTwoFactor;
using Qwestr.Modules.Identity.Infrastructure.Services;

namespace Qwestr.Modules.Identity.Api.Endpoints;

public static partial class IdentityEndpoints
{
    private const string RefreshTokenCookie = "refresh_token";

    private static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder identity)
    {
        var auth = identity.MapGroup("auth").WithTags("Identity.Auth");
        auth.MapPost("/register", RegisterHandler).AllowAnonymous();
        auth.MapPost("/login", LoginHandler).AllowAnonymous();
        auth.MapPost("/login/2fa", VerifyTwoFactorHandler).AllowAnonymous();
        auth.MapPost("/refresh", RefreshHandler).AllowAnonymous();
        auth.MapPost("/logout", LogoutHandler).RequireAuthorization();
        auth.MapPost("/password/forgot", ForgotPasswordHandler).AllowAnonymous();
        auth.MapPost("/password/reset", ResetPasswordHandler).AllowAnonymous();
        return identity;
    }

    private static async Task<IResult> LogoutHandler(HttpContext ctx, ISender sender, CancellationToken ct)
    {
        var refreshToken = ctx.Request.Cookies[RefreshTokenCookie];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await sender.Send(new LogoutCommand(refreshToken), ct);
        }

        DeleteRefreshTokenCookie(ctx);
        return Results.NoContent();
    }

    private static async Task<IResult> RefreshHandler(HttpContext ctx, ISender sender, 
        IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var refreshToken = ctx.Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Results.Unauthorized();
        }

        var result = await sender.Send(new RefreshTokenCommand(refreshToken), ct);
        if (!result.IsSuccess)
        {
            DeleteRefreshTokenCookie(ctx);
            return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: result.Error);
        }

        var data = result.Value!;
        SetRefreshTokenCookie(ctx, data.RefreshToken, jwtOptions);

        return Results.Ok(new { data.AccessToken, data.ExpiresAt });
    }

    private static async Task<IResult> LoginHandler(LoginCommand command, HttpContext ctx, 
        ISender sender, IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
        {
            return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: result.Error);
        }

        var data = result.Value!;
        if (!data.RequiresTwoFactor)
        {
            SetRefreshTokenCookie(ctx, data.RefreshToken, jwtOptions);
        }

        return Results.Ok(new { data.AccessToken, data.ExpiresAt, data.RequiresTwoFactor });
    }

    private static async Task<IResult> RegisterHandler(RegisterTenantCommand command, HttpContext ctx, 
        ISender sender, IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { result.Error });
        }

        var data = result.Value!;
        SetRefreshTokenCookie(ctx, data.RefreshToken, jwtOptions);

        return Results.Created($"/api/identity/tenant/{data.TenantId}", new { data.TenantId, data.AccessToken, data.ExpiresAt });
    }

    private static void SetRefreshTokenCookie(HttpContext ctx, string token, IOptions<JwtSettings> jwtOptions)
    {
        ctx.Response.Cookies.Append(RefreshTokenCookie, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays)
        });
    }

    private static async Task<IResult> VerifyTwoFactorHandler(VerifyTwoFactorCommand command, HttpContext ctx,
        ISender sender, IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
        {
            return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: result.Error);
        }

        var data = result.Value!;
        SetRefreshTokenCookie(ctx, data.RefreshToken, jwtOptions);

        return Results.Ok(new { data.AccessToken, data.ExpiresAt });
    }

    private static async Task<IResult> ForgotPasswordHandler(RequestPasswordResetCommand command, ISender sender, CancellationToken ct)
    {
        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> ResetPasswordHandler(ResetPasswordCommand command, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { result.Error });
    }

    private static void DeleteRefreshTokenCookie(HttpContext ctx)
    {
        ctx.Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
    }
}