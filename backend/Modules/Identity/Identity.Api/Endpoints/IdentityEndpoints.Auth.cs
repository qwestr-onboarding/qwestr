using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Identity.Application.Commands.Login;
using Qwestr.Modules.Identity.Application.Commands.Logout;
using Qwestr.Modules.Identity.Application.Commands.RegisterTenant;
using Qwestr.Modules.Identity.Application.Commands.RequestPasswordReset;
using Qwestr.Modules.Identity.Application.Commands.ResetPassword;
using Qwestr.Modules.Identity.Application.Commands.VerifyTwoFactor;
using Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;

namespace Qwestr.Modules.Identity.Api.Endpoints;

public static partial class IdentityEndpoints
{
    private static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder identity)
    {
        var auth = identity.MapGroup("auth").WithTags("Identity.Auth");
        auth.MapPost("/register", RegisterHandler).AllowAnonymous();
        auth.MapPost("/login", LoginHandler).AllowAnonymous();
        auth.MapPost("/login/2fa", VerifyTwoFactorHandler).AllowAnonymous();
        auth.MapPost("/logout", LogoutHandler).RequireAuthorization();
        auth.MapPost("/password/forgot", ForgotPasswordHandler).AllowAnonymous();
        auth.MapPost("/password/reset", ResetPasswordHandler).AllowAnonymous();
        return identity;
    }

    private static async Task<IResult> LoginHandler(LoginRequest dto, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(dto.Email, dto.Password, dto.Subdomain), ct);
        if (!result.IsSuccess)
        {
            return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: result.Error);
        }

        var data = result.Value!;
        return Results.Ok(new LoginResponse(data.AccessToken, data.ExpiresAt, data.RequiresTwoFactor));
    }

    private static async Task<IResult> RegisterHandler(RegisterTenantRequest dto, ISender sender, CancellationToken ct)
    {
        var command = new RegisterTenantCommand(dto.Name, dto.Subdomain, dto.OwnerEmail, dto.OwnerPassword,
            dto.OwnerFirstName, dto.OwnerLastName);
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new ErrorResponse(result.Error));
        }

        var data = result.Value!;
        return Results.Created($"/api/identity/tenant/{data.TenantId}",
            new RegisterResponse(data.TenantId, data.AccessToken, data.ExpiresAt));
    }

    private static async Task<IResult> VerifyTwoFactorHandler(VerifyTwoFactorRequest dto, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new VerifyTwoFactorCommand(dto.TempToken, dto.Code, dto.IsBackupCode), ct);
        if (!result.IsSuccess)
        {
            return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: result.Error);
        }

        var data = result.Value!;
        return Results.Ok(new VerifyTwoFactorResponse(data.AccessToken, data.ExpiresAt));
    }

    private static async Task<IResult> LogoutHandler(ISender sender, CancellationToken ct)
    {
        await sender.Send(new LogoutCommand(), ct);
        return Results.NoContent();
    }

    private static async Task<IResult> ForgotPasswordHandler(RequestPasswordResetRequest dto, ISender sender, CancellationToken ct)
    {
        await sender.Send(new RequestPasswordResetCommand(dto.Email, dto.Subdomain), ct);
        return Results.NoContent();
    }

    private static async Task<IResult> ResetPasswordHandler(ResetPasswordRequest dto, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new ResetPasswordCommand(dto.Token, dto.NewPassword), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
