using Qwestr.Modules.Analytics.Application.AI.Models;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.AI.Parsing;

public interface ILlmResponseParser
{
    ActionResult<LlmInsightResponse> Parse(string rawJson);
}
