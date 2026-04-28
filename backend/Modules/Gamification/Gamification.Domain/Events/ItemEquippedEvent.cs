using MediatR;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Domain.Events;

public sealed record ItemEquippedEvent(Guid TenantId, Guid ProfileId, Guid UserId, InventorySlot Slot, Guid ItemId) : INotification;