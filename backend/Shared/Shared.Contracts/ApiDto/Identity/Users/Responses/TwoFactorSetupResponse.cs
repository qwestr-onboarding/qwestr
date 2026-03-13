namespace Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;

public record TwoFactorSetupResponse(string Secret, string QrCodeUri);
