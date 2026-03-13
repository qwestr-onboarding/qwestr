namespace Client.Infrastructure.Models.Requests;

public record CreateInvitationRequest(string Email, string Role);

public record AcceptInvitationRequest(
    string Token,
    string Password,
    string FirstName,
    string LastName,
    string? Username);