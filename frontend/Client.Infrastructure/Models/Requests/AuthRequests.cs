namespace Client.Infrastructure.Models.Requests;

public record LoginRequest(string Email, string Password, string Subdomain);

public record VerifyTwoFactorRequest(string Code, bool IsBackupCode);

public record RegisterTenantRequest(
    string CompanyName,
    string Subdomain,
    string Email,
    string Password,
    string FirstName,
    string LastName);

public record ForgotPasswordRequest(string Email, string Subdomain);

public record ResetPasswordRequest(string Token, string NewPassword);