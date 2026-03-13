namespace Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Responses;

public record AcceptInvitationResponse(string AccessToken, DateTime ExpiresAt);