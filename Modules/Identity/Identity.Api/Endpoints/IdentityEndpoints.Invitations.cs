using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Identity.Application.Commands.AcceptInvitation;
using Qwestr.Modules.Identity.Application.Commands.CancelInvitation;
using Qwestr.Modules.Identity.Application.Commands.CreateInvitation;
using Qwestr.Modules.Identity.Application.Queries.GetInvitationsList;
using Qwestr.Modules.Identity.Domain.Enums;
using Qwestr.Modules.Identity.Infrastructure.Services;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Identity.Api.Endpoints;

internal record CreateInvitationRequest(string Email, UserRole Role);

public static partial class IdentityEndpoints
{
    private static IEndpointRouteBuilder MapInvitationsEndpoints(this IEndpointRouteBuilder identity)
    {
        var invitations = identity.MapGroup("invitations").WithTags("Identity.Invitations");

        invitations.MapPost("/", CreateInvitationHandler)
            .RequireAuthorization(p => p.RequireRole("Admin", "Owner"));
        invitations.MapGet("/", GetInvitationsListHandler)
            .RequireAuthorization(p => p.RequireRole("Admin", "Owner"));
        invitations.MapPost("/accept", AcceptInvitationHandler)
            .AllowAnonymous();
        invitations.MapDelete("/{id:guid}", CancelInvitationHandler)
            .RequireAuthorization(p => p.RequireRole("Admin", "Owner"));

        return identity;
    }

    private static async Task<IResult> CreateInvitationHandler(
        CreateInvitationRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateInvitationCommand(currentUser.TenantId, currentUser.UserId, body.Email, body.Role), ct);
        return result.IsSuccess
            ? Results.Created($"/api/identity/invitations/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new { result.Error });
    }

    private static async Task<IResult> GetInvitationsListHandler(
        int page, int pageSize, InvitationStatus? status,
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new GetInvitationsListQuery(currentUser.TenantId, page, pageSize, status), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { result.Error });
    }

    private static async Task<IResult> AcceptInvitationHandler(
        AcceptInvitationCommand command, HttpContext ctx,
        ISender sender, IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { result.Error });
        }

        var data = result.Value!;
        SetRefreshTokenCookie(ctx, data.RefreshToken, jwtOptions);

        return Results.Ok(new { data.AccessToken, data.ExpiresAt });
    }

    private static async Task<IResult> CancelInvitationHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new CancelInvitationCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { result.Error });
    }
}
