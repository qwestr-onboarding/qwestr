using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Notifications.Application.Commands.MarkAllAsRead;
using Qwestr.Modules.Notifications.Application.Commands.MarkAsRead;
using Qwestr.Modules.Notifications.Application.Queries.GetUnreadCount;
using Qwestr.Modules.Notifications.Application.Queries.GetUserNotifications;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Notifications.Api.Endpoints;

public static partial class NotificationsEndpoints
{
    private static IEndpointRouteBuilder MapNotificationsListEndpoints(this IEndpointRouteBuilder group)
    {
        group.MapGet("/", GetNotificationsHandler);
        group.MapGet("/unread-count", GetUnreadCountHandler);
        group.MapPost("/{id:guid}/mark-read", MarkAsReadHandler);
        group.MapPost("/mark-all-read", MarkAllAsReadHandler);

        return group;
    }

    private static async Task<IResult> GetNotificationsHandler(
        ICurrentUserService currentUser,
        ISender sender,
        bool unreadOnly = false,
        int take = 20,
        int skip = 0,
        CancellationToken ct = default)
    {
        if (take > 100)
        {
            take = 100;
        }

        var result = await sender.Send(
            new GetUserNotificationsQuery(currentUser.TenantId, currentUser.UserId, unreadOnly, take, skip), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetUnreadCountHandler(
        ICurrentUserService currentUser,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new GetUnreadCountQuery(currentUser.TenantId, currentUser.UserId), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> MarkAsReadHandler(
        Guid id,
        ICurrentUserService currentUser,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new MarkAsReadCommand(currentUser.TenantId, currentUser.UserId, id), ct);

        if (result.IsFailure && result.Error?.Contains("not_found") == true)
        {
            return Results.NotFound(new ErrorResponse(result.Error));
        }

        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> MarkAllAsReadHandler(
        ICurrentUserService currentUser,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new MarkAllAsReadCommand(currentUser.TenantId, currentUser.UserId), ct);

        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
