using MediatR;

namespace Qwestr.Shared.Contracts.Identity.Events;

public record UserActivatedIntegrationEvent(
    Guid TenantId,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string CompanyName) : INotification;
