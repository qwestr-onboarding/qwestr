using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Identity.Application.Commands.UpdateTenantSettings;
using Qwestr.Modules.Identity.Application.Queries.GetTenantSettings;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Identity.Api.Endpoints;

internal record UpdateTenantSettingsRequest(string? Timezone, string? Language, string? CompanyLogoUrl);

public static partial class IdentityEndpoints
{
    private static IEndpointRouteBuilder MapTenantSettingsEndpoints(this IEndpointRouteBuilder identity)
    {
        var settings = identity.MapGroup("tenant/settings").WithTags("Identity.Tenant");

        settings.MapGet("/", GetTenantSettingsHandler)
            .RequireAuthorization();
        settings.MapPut("/", UpdateTenantSettingsHandler)
            .RequireAuthorization(p => p.RequireRole("Admin", "Owner"));

        return identity;
    }

    private static async Task<IResult> GetTenantSettingsHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetTenantSettingsQuery(currentUser.TenantId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { result.Error });
    }

    private static async Task<IResult> UpdateTenantSettingsHandler(
        UpdateTenantSettingsRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateTenantSettingsCommand(currentUser.TenantId, body.Timezone, body.Language, body.CompanyLogoUrl), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { result.Error });
    }
}
