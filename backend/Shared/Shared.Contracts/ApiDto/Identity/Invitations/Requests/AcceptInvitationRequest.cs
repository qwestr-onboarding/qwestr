namespace Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Requests;

public record AcceptInvitationRequest(string Token,
    string Password,
    string FirstName,
    string LastName);