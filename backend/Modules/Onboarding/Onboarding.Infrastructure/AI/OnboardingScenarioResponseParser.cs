using System.Text.Json;
using System.Text.Json.Nodes;
using Qwestr.Modules.Onboarding.Application.AI.Models;
using Qwestr.Shared.Kernel.Enums.Onboarding;
using Serilog;

namespace Qwestr.Modules.Onboarding.Infrastructure.AI;

public class OnboardingScenarioResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger _logger = Log.ForContext<OnboardingScenarioResponseParser>();

    public GeneratedScenarioDraft Parse(string raw)
    {
        var json = StripMarkdownWrapper(raw.Trim());

        try
        {
            var root = JsonNode.Parse(json)?.AsObject()
                ?? throw new JsonException("Root is not a JSON object");

            var title = root["suggestedTitle"]?.GetValue<string>() ?? "Сгенерированный сценарий";
            var description = root["suggestedDescription"]?.GetValue<string>();
            var aiComment = root["aiComment"]?.GetValue<string>() ?? string.Empty;

            var stages = new List<GeneratedStageDraft>();
            var stagesArray = root["stages"]?.AsArray();
            if (stagesArray is not null)
            {
                foreach (var stageNode in stagesArray)
                {
                    var stage = ParseStage(stageNode?.AsObject());
                    if (stage is not null) stages.Add(stage);
                }
            }

            return new GeneratedScenarioDraft(title, description, stages, aiComment);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to parse LLM response. Raw: {Raw}", raw);
            throw new LlmParsingException("Failed to parse onboarding scenario from LLM response", ex);
        }
    }

    private static GeneratedStageDraft? ParseStage(JsonObject? node)
    {
        if (node is null) return null;

        var title = node["title"]?.GetValue<string>() ?? "Этап";
        var description = node["description"]?.GetValue<string>();
        var steps = new List<GeneratedStepDraft>();

        var stepsArray = node["steps"]?.AsArray();
        if (stepsArray is not null)
        {
            foreach (var stepNode in stepsArray)
            {
                var step = ParseStep(stepNode?.AsObject());
                if (step is not null) steps.Add(step);
            }
        }

        return new GeneratedStageDraft(title, description, steps);
    }

    private static GeneratedStepDraft? ParseStep(JsonObject? node)
    {
        if (node is null) return null;

        var typeStr = node["type"]?.GetValue<string>() ?? "information";
        var type = ParseStepType(typeStr);
        var title = node["title"]?.GetValue<string>() ?? "Шаг";
        var points = node["points"]?.GetValue<int?>();

        string? contentJson = null;
        var contentNode = node["content"];
        if (contentNode is not null)
        {
            if (type == StepType.Information)
                contentJson = SerializeInfoBlocks(contentNode);
            else if (type == StepType.Checklist)
                contentJson = SerializeChecklistItems(contentNode);
            else if (type == StepType.Task)
                contentJson = NormalizeTaskContent(contentNode);
            else
                contentJson = contentNode.ToJsonString();
        }

        List<GeneratedQuestionDraft>? questions = null;
        var questionsArray = node["questions"]?.AsArray();
        if (questionsArray is not null && questionsArray.Count > 0)
        {
            questions = [];
            foreach (var qNode in questionsArray)
            {
                var q = ParseQuestion(qNode?.AsObject());
                if (q is not null) questions.Add(q);
            }
        }

        // For question/test steps, questions may come in several shapes from the LLM:
        //   1. content.questions: [...] — preferred format for test steps (multiple questions)
        //   2. content as JsonArray of {question: {...}} objects
        //   3. step.question or content.question — single-question fallback
        if (questions is null && type is StepType.Question or StepType.Test)
        {
            questions = [];

            // Shape 1: content is an object with a "questions" array (test steps)
            var contentQuestionsArray = (contentNode as JsonObject)?["questions"]?.AsArray();
            if (contentQuestionsArray is not null)
            {
                foreach (var qNode in contentQuestionsArray)
                {
                    var q = ParseQuestion(qNode as JsonObject);
                    if (q is not null) questions.Add(q);
                }
            }
            // Shape 2: content itself is an array of {question: {...}} wrappers
            else if (contentNode is JsonArray contentArray)
            {
                foreach (var item in contentArray)
                {
                    if (item is not JsonObject itemObj) continue;
                    var qNode = itemObj["question"] as JsonObject;
                    if (qNode is not null)
                    {
                        var q = ParseQuestion(qNode);
                        if (q is not null) questions.Add(q);
                    }
                }
            }
            // Shape 3: single question at step root or inside content object
            else
            {
                var questionNode = node["question"] as JsonObject
                    ?? (contentNode as JsonObject)?["question"] as JsonObject;
                if (questionNode is not null)
                {
                    var q = ParseQuestion(questionNode);
                    if (q is not null) questions.Add(q);
                }
            }
        }

        return new GeneratedStepDraft(type, title, points, contentJson, null, questions);
    }

    private static GeneratedQuestionDraft? ParseQuestion(JsonObject? node)
    {
        if (node is null) return null;

        var typeStr = node["type"]?.GetValue<string>() ?? "singleChoice";
        var questionType = typeStr switch
        {
            "multipleChoice" => QuestionType.MultipleChoice,
            "openText" => QuestionType.OpenText,
            _ => QuestionType.SingleChoice
        };

        var text = node["text"]?.GetValue<string>()
            ?? node["questionText"]?.GetValue<string>()
            ?? "Вопрос";

        var options = new List<GeneratedOptionDraft>();
        var optionsArray = node["options"]?.AsArray();
        if (optionsArray is not null)
        {
            foreach (var optNode in optionsArray)
            {
                if (optNode is JsonObject optObj)
                {
                    var optText = optObj["text"]?.GetValue<string>() ?? string.Empty;
                    var isCorrect = optObj["isCorrect"]?.GetValue<bool>() ?? false;
                    options.Add(new GeneratedOptionDraft(optText, isCorrect));
                }
                else if (optNode is JsonValue optVal && optVal.TryGetValue<string>(out var optStr))
                {
                    // Model returned plain strings instead of {text, isCorrect} objects
                    options.Add(new GeneratedOptionDraft(optStr, false));
                }
            }
        }

        return new GeneratedQuestionDraft(questionType, text, options);
    }

    // Normalises information-step content to the flat array format the frontend expects,
    // handling both the correct flat-array format and the legacy {"blocks":[...]} wrapper.
    // Adds "order" to each block and normalises field names (level int→"h1"/"h2", markdown→content).
    private static string? SerializeInfoBlocks(JsonNode contentNode)
    {
        var blocksArray = contentNode as JsonArray
            ?? (contentNode as JsonObject)?["blocks"]?.AsArray();

        if (blocksArray is null)
            return contentNode.ToJsonString();

        var result = new JsonArray();
        for (var i = 0; i < blocksArray.Count; i++)
        {
            if (blocksArray[i] is not JsonObject src) continue;

            var block = new JsonObject();
            block["type"] = src["type"]?.GetValue<string>();
            block["order"] = i;

            var blockType = src["type"]?.GetValue<string>() ?? "";

            if (blockType == "heading")
            {
                // level: int 1/2 → string "h1"/"h2"
                var levelNode = src["level"];
                string levelStr;
                if (levelNode is JsonValue lv && lv.TryGetValue<int>(out var li))
                    levelStr = li <= 1 ? "h1" : "h2";
                else
                    levelStr = levelNode?.GetValue<string>() is "h1" ? "h1" : "h2";
                block["level"] = levelStr;
                block["content"] = src["content"]?.GetValue<string>()
                    ?? src["text"]?.GetValue<string>() ?? string.Empty;
            }
            else if (blockType == "text")
            {
                // field may be "content" or legacy "markdown"
                block["content"] = src["content"]?.GetValue<string>()
                    ?? src["markdown"]?.GetValue<string>() ?? string.Empty;
            }
            else if (blockType == "callout")
            {
                block["content"] = src["content"]?.GetValue<string>() ?? string.Empty;
                block["variant"] = src["variant"]?.GetValue<string>() ?? "information";
            }
            else if (blockType == "image")
            {
                block["content"] = src["content"]?.GetValue<string>() ?? string.Empty;
                if (src["isPlaceholder"]?.GetValue<bool>() == true)
                    block["isPlaceholder"] = true;
            }
            else if (blockType == "video")
            {
                block["content"] = src["content"]?.GetValue<string>() ?? string.Empty;
            }
            else if (blockType == "footnote")
            {
                block["content"] = src["content"]?.GetValue<string>()
                    ?? src["text"]?.GetValue<string>() ?? string.Empty;
            }
            // "divider" needs no extra fields

            result.Add(block);
        }

        return result.ToJsonString();
    }

    // AI returns {"description":"...","hints":["..."]} but the UI stores {"text":"..."}
    private static string NormalizeTaskContent(JsonNode contentNode)
    {
        if (contentNode is not JsonObject obj)
            return contentNode.ToJsonString();

        var text = obj["description"]?.GetValue<string>()
            ?? obj["text"]?.GetValue<string>()
            ?? string.Empty;

        var hintsArray = obj["hints"]?.AsArray();
        if (hintsArray is { Count: > 0 })
        {
            var hints = string.Join("\n", hintsArray
                .Select(h => h?.GetValue<string>())
                .Where(h => !string.IsNullOrEmpty(h))
                .Select(h => $"• {h}"));
            if (!string.IsNullOrEmpty(hints))
                text = string.IsNullOrEmpty(text) ? hints : $"{text}\n\n{hints}";
        }

        var result = new JsonObject { ["text"] = text };
        return result.ToJsonString();
    }

    private static string SerializeChecklistItems(JsonNode contentNode)
    {
        if (contentNode is JsonArray arr)
            return arr.ToJsonString();

        if (contentNode is JsonObject obj)
        {
            var items = obj["items"]?.AsArray();
            if (items is not null)
                return items.ToJsonString();
        }

        return contentNode.ToJsonString();
    }

    private static StepType ParseStepType(string raw) => raw.ToLowerInvariant() switch
    {
        "question" => StepType.Question,
        "test" => StepType.Test,
        "task" => StepType.Task,
        "checklist" => StepType.Checklist,
        _ => StepType.Information
    };

    private static string StripMarkdownWrapper(string raw)
    {
        if (raw.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            raw = raw["```json".Length..].TrimStart();
        else if (raw.StartsWith("```"))
            raw = raw["```".Length..].TrimStart();

        if (raw.EndsWith("```"))
            raw = raw[..^3].TrimEnd();

        return raw;
    }
}

public class LlmParsingException : Exception
{
    public LlmParsingException(string message, Exception? inner = null) : base(message, inner) { }
}
