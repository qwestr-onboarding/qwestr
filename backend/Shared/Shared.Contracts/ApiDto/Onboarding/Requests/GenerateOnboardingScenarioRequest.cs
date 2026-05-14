using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record GenerateOnboardingScenarioRequest(
    string Query,
    List<AttachedDocumentDto>? Documents,
    int StageCount,
    int StepsPerStage,
    string TextStyle,
    string? AdditionalInfo,
    string? TargetAudience,
    bool IncludeFinalTest,
    int? TotalPoints,
    LlmProvider? Provider);

public record AttachedDocumentDto(string FileName, string ContentBase64, string MimeType);
