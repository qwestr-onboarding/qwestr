using MediatR;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Shared.Contracts.Identity.Events;

public record UserRegisteredIntegrationEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    string? Username,
    UserRole Role) : INotification;