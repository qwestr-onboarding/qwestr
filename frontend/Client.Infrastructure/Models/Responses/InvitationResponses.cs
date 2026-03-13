namespace Client.Infrastructure.Models.Responses;

public record InvitationResponse(
    Guid Id,
    string Email,
    string Role,
    string Status,
    string InvitedByName,
    DateTime CreatedAt,
    DateTime ExpiresAt);