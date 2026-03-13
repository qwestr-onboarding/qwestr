namespace Client.Infrastructure.Models.Requests;

public record UpdateProfileRequest(string FirstName, string LastName, string? Username);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record ConfirmTwoFactorRequest(string Code);

public record DisableTwoFactorRequest(string Code);

public record ChangeRoleRequest(string Role);