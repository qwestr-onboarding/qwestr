using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Notifications.Application.Commands.UpdatePreferences;
using Qwestr.Modules.Notifications.Application.Queries.GetUserPreferences;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Api.Endpoints;

public static partial class NotificationsEndpoints
{
    private static IEndpointRouteBuilder MapPreferencesEndpoints(this IEndpointRouteBuilder group)
    {
        group.MapGet("/preferences", GetPreferencesHandler);
        group.MapPut("/preferences", UpdatePreferencesHandler);

        return group;
    }

    private static async Task<IResult> GetPreferencesHandler(
        ICurrentUserService currentUser,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new GetUserPreferencesQuery(currentUser.TenantId, currentUser.UserId), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdatePreferencesHandler(
        UpdatePreferencesRequest body,
        ICurrentUserService currentUser,
        ISender sender,
        CancellationToken ct = default)
    {
        var items = body.Preferences
            .Select(p => new PreferenceItem((NotificationType)p.Type, p.InAppEnabled, p.EmailEnabled))
            .ToList();

        var result = await sender.Send(
            new UpdatePreferencesCommand(currentUser.TenantId, currentUser.UserId, items), ct);

        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
