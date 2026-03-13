using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Identity.Application.Commands.AcceptInvitation;
using Qwestr.Modules.Identity.Application.Commands.CancelInvitation;
using Qwestr.Modules.Identity.Application.Commands.CreateInvitation;
using Qwestr.Modules.Identity.Application.Queries.GetInvitationsList;
using Qwestr.Modules.Identity.Infrastructure.Services;
using Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Responses;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Api.Endpoints;

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
            ? Results.Created($"/api/identity/invitations/{result.Value}", new CreateInvitationResponse(result.Value))
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetInvitationsListHandler(
        [FromQuery] int page, [FromQuery] int pageSize, [FromQuery] InvitationStatus? status,
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new GetInvitationsListQuery(currentUser.TenantId, page, pageSize, status), ct);
        return result.IsSuccess
            ? Results.Ok(new PagedListResponse<InvitationResponse>(result.Value!))
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> AcceptInvitationHandler(
        AcceptInvitationRequest body, HttpContext ctx,
        ISender sender, IOptions<JwtSettings> jwtOptions, CancellationToken ct)
    {
        var command = new AcceptInvitationCommand(body.Token, body.Password, body.FirstName, body.LastName);
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new ErrorResponse(result.Error));
        }

        var data = result.Value!;
        SetRefreshTokenCookie(ctx, data.RefreshToken, jwtOptions);

        return Results.Ok(new AcceptInvitationResponse(data.AccessToken, data.ExpiresAt));
    }

    private static async Task<IResult> CancelInvitationHandler(
        [FromRoute] Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new CancelInvitationCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
