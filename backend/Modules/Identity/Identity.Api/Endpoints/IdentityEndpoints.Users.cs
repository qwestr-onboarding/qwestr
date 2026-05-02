using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Identity.Application.Commands.ActivateUser;
using Qwestr.Modules.Identity.Application.Commands.ChangePassword;
using Qwestr.Modules.Identity.Application.Commands.ChangeUserRole;
using Qwestr.Modules.Identity.Application.Commands.ConfirmTwoFactor;
using Qwestr.Modules.Identity.Application.Commands.DeactivateUser;
using Qwestr.Modules.Identity.Application.Commands.DeleteAvatar;
using Qwestr.Modules.Identity.Application.Commands.DisableTwoFactor;
using Qwestr.Modules.Identity.Application.Commands.SetupTwoFactor;
using Qwestr.Modules.Identity.Application.Commands.UpdateProfile;
using Qwestr.Modules.Identity.Application.Commands.UploadAvatar;
using Qwestr.Modules.Identity.Application.Queries.GetCurrentUser;
using Qwestr.Modules.Identity.Application.Queries.GetUserById;
using Qwestr.Modules.Identity.Application.Queries.GetUsersList;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Identity.Api.Endpoints;

public static partial class IdentityEndpoints
{
    private static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder identity)
    {
        var users = identity.MapGroup("users").WithTags("Identity.Users").RequireAuthorization();

        users.MapGet("/me", GetCurrentUserHandler);
        users.MapPut("/me", UpdateProfileHandler);
        users.MapPut("/me/password", ChangePasswordHandler);
        users.MapPost("/me/avatar", UploadAvatarHandler).DisableAntiforgery();
        users.MapDelete("/me/avatar", DeleteAvatarHandler);
        users.MapPost("/me/2fa/setup", SetupTwoFactorHandler);
        users.MapPost("/me/2fa/confirm", ConfirmTwoFactorHandler);
        users.MapDelete("/me/2fa", DisableTwoFactorHandler);

        users.MapGet("/", GetUsersListHandler)
            .RequireAuthorization(p => p.RequireRole("Manager", "Admin", "Owner"));
        users.MapGet("/{id:guid}", GetUserByIdHandler);
        users.MapPut("/{id:guid}/deactivate", DeactivateUserHandler)
            .RequireAuthorization(p => p.RequireRole("Admin", "Owner"));
        users.MapPut("/{id:guid}/activate", ActivateUserHandler)
            .RequireAuthorization(p => p.RequireRole("Admin", "Owner"));
        users.MapPut("/{id:guid}/role", ChangeUserRoleHandler)
            .RequireAuthorization(p => p.RequireRole("Owner"));

        return identity;
    }

    private static async Task<IResult> GetCurrentUserHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetCurrentUserQuery(currentUser.UserId), ct);
        var response = result.Value;
        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateProfileHandler(
        UpdateProfileRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateProfileCommand(currentUser.UserId, body.FirstName, body.LastName), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> ChangePasswordHandler(
        ChangePasswordRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new ChangePasswordCommand(currentUser.UserId, body.CurrentPassword, body.NewPassword), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UploadAvatarHandler(
        IFormFile file, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var result = await sender.Send(
            new UploadAvatarCommand(currentUser.UserId, stream, file.Length, file.ContentType), ct);
        return result.IsSuccess
            ? Results.Ok(new UploadAvatarResponse(result.Value!))
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeleteAvatarHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteAvatarCommand(currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> SetupTwoFactorHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new SetupTwoFactorCommand(currentUser.UserId), ct);
        var response = result.Value;
        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> ConfirmTwoFactorHandler(
        ConfirmTwoFactorRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new ConfirmTwoFactorCommand(currentUser.UserId, body.Code), ct);
        var response = result.Value;
        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DisableTwoFactorHandler(
        [FromBody] DisableTwoFactorRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DisableTwoFactorCommand(currentUser.UserId, body.Code), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetUsersListHandler(
        int page, int pageSize, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetUsersListQuery(currentUser.TenantId, page, pageSize), ct);
        var response = new PagedListResponse<UserListItemResponse>(result.Value!);
        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetUserByIdHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByIdQuery(currentUser.TenantId, id), ct);
        var response = result.Value;
        return result.IsSuccess
            ? Results.Ok(response)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeactivateUserHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeactivateUserCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> ActivateUserHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new ActivateUserCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> ChangeUserRoleHandler(
        Guid id, ChangeUserRoleRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new ChangeUserRoleCommand(currentUser.TenantId, currentUser.UserId, id, body.NewRole), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
