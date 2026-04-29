using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Gamification.Application.Inventory.Commands.EquipItem;
using Qwestr.Modules.Gamification.Application.Inventory.Commands.UnequipItem;
using Qwestr.Modules.Gamification.Application.Inventory.Queries.GetMyInventory;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Api.Endpoints;

public static partial class GamificationEndpoints
{
    private static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder gamification)
    {
        var inventory = gamification.MapGroup("inventory").WithTags("Gamification.Inventory").RequireAuthorization();

        inventory.MapGet("/me", GetMyInventoryHandler);
        inventory.MapPost("/equip", EquipItemHandler);
        inventory.MapPost("/unequip", UnequipItemHandler);

        return gamification;
    }

    private static async Task<IResult> GetMyInventoryHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetMyInventoryQuery(currentUser.TenantId, currentUser.UserId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> EquipItemHandler(
        EquipItemRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new EquipItemCommand(currentUser.TenantId, currentUser.UserId, body.ShopItemId), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> UnequipItemHandler(
        UnequipItemRequest body, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        if (!Enum.TryParse<InventorySlot>(body.Slot, true, out var slot))
        {
            return Results.BadRequest(new ErrorResponse($"Unknown slot: {body.Slot}"));
        }

        var result = await sender.Send(new UnequipItemCommand(currentUser.TenantId, currentUser.UserId, slot), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
