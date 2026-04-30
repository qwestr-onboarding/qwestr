using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Shared.Contracts.ApiDto.Analytics.Requests;

public record GenerateInsightsRequest(LlmProvider Provider);
