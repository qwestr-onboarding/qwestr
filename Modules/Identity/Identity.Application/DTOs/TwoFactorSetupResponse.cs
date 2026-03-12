namespace Qwestr.Modules.Identity.Application.DTOs;

public record TwoFactorSetupResponse(string Secret, string QrCodeUri);
