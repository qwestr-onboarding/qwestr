using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Gamification.Application.Shop.Commands.CreateShopItem;
using Qwestr.Modules.Gamification.Application.Shop.Commands.DeactivateShopItem;
using Qwestr.Modules.Gamification.Application.Shop.Commands.PurchaseShopItem;
using Qwestr.Modules.Gamification.Application.Shop.Commands.UpdateShopItem;
using Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItemById;
using Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItems;
using Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItemsForManagement;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Api.Endpoints;

public static partial class GamificationEndpoints
{
    private static IEndpointRouteBuilder MapShopEndpoints(this IEndpointRouteBuilder gamification)
    {
        var shop = gamification.MapGroup("shop").WithTags("Gamification.Shop");

        shop.MapGet("/", GetShopItemsHandler).RequireAuthorization();
        shop.MapGet("/{id:guid}", GetShopItemByIdHandler).RequireAuthorization();
        shop.MapGet("/manage", GetShopItemsForManagementHandler).RequireAuthorization(HrPolicy);
        shop.MapPost("/", CreateShopItemHandler).RequireAuthorization(HrPolicy);
        shop.MapPut("/{id:guid}", UpdateShopItemHandler).RequireAuthorization(HrPolicy);
        shop.MapPost("/{id:guid}/deactivate", DeactivateShopItemHandler).RequireAuthorization(HrPolicy);
        shop.MapPost("/{id:guid}/purchase", PurchaseShopItemHandler).RequireAuthorization();

        return gamification;
    }

    private static async Task<IResult> GetShopItemsHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetShopItemsQuery(currentUser.TenantId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetShopItemByIdHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetShopItemByIdQuery(currentUser.TenantId, currentUser.UserId, id), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetShopItemsForManagementHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetShopItemsForManagementQuery(currentUser.TenantId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> CreateShopItemHandler(
        CreateShopItemRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        if (!Enum.TryParse<InventorySlot>(body.Slot, true, out var slot))
        {
            return Results.BadRequest(new ErrorResponse($"Unknown slot: {body.Slot}"));
        }

        if (!Enum.TryParse<StackingPolicy>(body.StackingPolicy, true, out var policy))
        {
            return Results.BadRequest(new ErrorResponse($"Unknown stacking policy: {body.StackingPolicy}"));
        }

        var result = await sender.Send(
            new CreateShopItemCommand(
                currentUser.TenantId, slot, body.Name, body.Description,
                body.IconUrl, body.AssetData, body.Price, body.RequiredLevel, policy), ct);
        return result.IsSuccess
            ? Results.Created($"/api/gamification/shop/{result.Value}", new { id = result.Value })
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UpdateShopItemHandler(
        Guid id, UpdateShopItemRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateShopItemCommand(
                currentUser.TenantId, id, body.Name, body.Description,
                body.IconUrl, body.AssetData, body.Price, body.RequiredLevel), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> DeactivateShopItemHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeactivateShopItemCommand(currentUser.TenantId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> PurchaseShopItemHandler(
        Guid id, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new PurchaseShopItemCommand(currentUser.TenantId, currentUser.UserId, id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
