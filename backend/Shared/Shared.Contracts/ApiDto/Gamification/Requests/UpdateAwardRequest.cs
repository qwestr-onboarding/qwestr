namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;

public record UpdateAwardRequest(
    string Title,
    string Description,
    string IconUrl);
