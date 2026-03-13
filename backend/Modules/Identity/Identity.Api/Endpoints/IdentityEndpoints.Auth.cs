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
using Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;

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

        return Results.Ok(new RefreshResponse(data.AccessToken, data.ExpiresAt));
    }

    private static async Task<IResult> LoginHandler(LoginRequest dto, HttpContext ctx, 
        ISender sender, IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var command = new LoginCommand(dto.Email, dto.Password, dto.Subdomain);
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

        return Results.Ok(new LoginResponse(data.AccessToken, data.ExpiresAt, data.RequiresTwoFactor));
    }

    private static async Task<IResult> RegisterHandler(RegisterTenantRequest dto, HttpContext ctx, 
        ISender sender, IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var command = new RegisterTenantCommand(dto.Name, dto.Subdomain, dto.OwnerEmail, dto.OwnerPassword,
            dto.OwnerFirstName, dto.OwnerLastName);
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new ErrorResponse(result.Error));
        }

        var data = result.Value!;
        SetRefreshTokenCookie(ctx, data.RefreshToken, jwtOptions);

        return Results.Created($"/api/identity/tenant/{data.TenantId}", 
            new RegisterResponse(data.TenantId, data.AccessToken, data.ExpiresAt));
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

    private static async Task<IResult> VerifyTwoFactorHandler(VerifyTwoRequest dto, HttpContext ctx,
        ISender sender, IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var command = new VerifyTwoFactorCommand(dto.TempToken, dto.Code, dto.IsBackupCode);
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
        {
            return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: result.Error);
        }

        var data = result.Value!;
        SetRefreshTokenCookie(ctx, data.RefreshToken, jwtOptions);

        return Results.Ok(new VerifyTwoFactorResponse(data.AccessToken, data.ExpiresAt));
    }

    private static async Task<IResult> ForgotPasswordHandler(RequestPasswordResetRequest dto, ISender sender, CancellationToken ct)
    {
        var command = new RequestPasswordResetCommand(dto.Email, dto.Subdomain);
        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> ResetPasswordHandler(ResetPasswordRequest dto, ISender sender, CancellationToken ct)
    {
        var command = new ResetPasswordCommand(dto.Token, dto.NewPassword);
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
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