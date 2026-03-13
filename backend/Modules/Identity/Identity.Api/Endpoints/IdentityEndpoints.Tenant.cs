using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Identity.Application.Commands.UpdateTenant;
using Qwestr.Modules.Identity.Application.Commands.UpdateTenantSettings;
using Qwestr.Modules.Identity.Application.Queries.GetTenant;
using Qwestr.Modules.Identity.Application.Queries.GetTenantBySubdomain;
using Qwestr.Modules.Identity.Application.Queries.GetTenantSettings;
using Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Identity.Api.Endpoints;

public static partial class IdentityEndpoints
{
    private static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder identity)
    {
        var tenant = identity.MapGroup("tenant").WithTags("Identity.Tenant").RequireAuthorization();

        tenant.MapGet("/lookup/{subdomain}", LookupTenantBySubdomainHandler).AllowAnonymous();
        tenant.MapGet("/", GetTenantHandler);
        tenant.MapPut("/", UpdateTenantHandler)
            .RequireAuthorization(p => p.RequireRole("Owner"));
        
        tenant.MapGet("/settings", GetTenantSettingsHandler)
            .RequireAuthorization();
        tenant.MapPut("/settings", UpdateTenantSettingsHandler)
            .RequireAuthorization(p => p.RequireRole("Admin", "Owner"));
        return identity;
    }

    private static async Task<IResult> LookupTenantBySubdomainHandler(
        string subdomain, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetTenantBySubdomainQuery(subdomain), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetTenantHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetTenantQuery(currentUser.TenantId), ct);
        var response = result.Value;
        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateTenantHandler(
        UpdateTenantRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateTenantCommand(currentUser.TenantId, body.Name, body.Subdomain), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
    
    private static async Task<IResult> GetTenantSettingsHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetTenantSettingsQuery(currentUser.TenantId), ct);
        var response = result.Value;
        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateTenantSettingsHandler(
        UpdateTenantSettingsRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateTenantSettingsCommand(currentUser.TenantId, body.Timezone, body.Language, body.CompanyLogoUrl), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
