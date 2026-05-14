using System.Text;
using Qwestr.Modules.Onboarding.Application.AI.Models;

namespace Qwestr.Modules.Onboarding.Infrastructure.AI;

public class OnboardingScenarioPromptBuilder(DocumentTextExtractor extractor)
{
    private readonly DocumentTextExtractor _extractor = extractor;
    private const string SystemPromptBase = """
        Ты — эксперт по разработке программ онбординга для сотрудников.
        Твоя задача — сгенерировать структурированный сценарий онбординга на основе запроса пользователя.

        СТРОГИЕ ПРАВИЛА:
        1. Отвечай ТОЛЬКО валидным JSON без каких-либо дополнительных символов, markdown-обёрток (```json) или пояснений.
        2. Следуй точно предоставленной JSON-схеме.
        3. Генерируй контент на том же языке, что и запрос пользователя.
        4. Для шагов типа "information" создавай богатый контент с блоками разных типов.
        5. Для шагов типа "question" и "test" создавай чёткие вопросы с вариантами ответов (минимум 2 варианта).
        6. В поле "aiComment" указывай конкретные рекомендации: какие реальные изображения, видео, ссылки или специфические данные компании стоит добавить вручную.

        ТИПЫ ШАГОВ:
        - "information": учебный материал. Поле "content" — ПЛОСКИЙ МАССИВ блоков (не объект с полем "blocks", а именно массив!).
          Поддерживаемые типы блоков:
          - {"type": "heading", "level": "h1" | "h2", "content": "..."}  ← level — СТРОКА "h1" или "h2", поле — "content"
          - {"type": "text", "content": "..."}  ← markdown-текст в поле "content" (не "markdown", не "text")
          - {"type": "divider"}
          - {"type": "callout", "content": "...", "variant": "information" | "warning" | "success"}
          - {"type": "image", "content": "", "isPlaceholder": true}  ← для заглушек; реальный URL пользователь добавит сам
          НЕ добавляй поле "order" — оно проставляется автоматически.
        - "question": один вопрос. Поле "content" содержит объект "question":
          {"type": "singleChoice" | "multipleChoice", "text": "...", "options": [{"text": "...", "isCorrect": true}, ...]}
          ВАЖНО: каждый элемент "options" — ОБЪЕКТ с полями "text" (string) и "isCorrect" (boolean). Никогда не используй массив строк для options.
        - "test": финальная проверка усвоенного материала этапа. Поле "content" содержит объект "questions" — МАССИВ из МИНИМУМ 3 вопросов. Каждый вопрос — объект с полями "type", "text" и "options" (аналогично "question"). ВАЖНО: это именно массив вопросов, а не один объект!
        - "checklist": список задач. Поле "content" — ПЛОСКИЙ МАССИВ строк: ["пункт 1", "пункт 2"]. Не оборачивай в объект {"items": [...]}, просто массив.
        - "task": практическое задание. Поле "content" — объект с одним полем: {"text": "..."}. Опиши задание подробно в этом поле, включая подсказки и инструкции прямо в тексте.

        СТИЛИ ТЕКСТА:
        - "friendly": дружелюбный, неформальный тон, использование "ты", приветственные фразы
        - "strict": официальный, деловой тон, использование "вы", чёткие инструкции
        - "simple": максимально простой язык, короткие предложения, минимум терминологии
        """;

    private const string ResponseSchema = """
        {
          "suggestedTitle": "string",
          "suggestedDescription": "string | null",
          "stages": [
            {
              "title": "string",
              "description": "string | null",
              "steps": [
                {
                  "type": "information",
                  "title": "string",
                  "points": null,
                  "content": [
                    {"type": "heading", "level": "h1", "content": "string"},
                    {"type": "text", "content": "string с **markdown**"},
                    {"type": "divider"},
                    {"type": "callout", "content": "string", "variant": "information"},
                    {"type": "image", "content": "", "isPlaceholder": true}
                  ]
                },
                {
                  "type": "question",
                  "title": "string",
                  "points": "number | null",
                  "content": {
                    "question": {
                      "type": "singleChoice",
                      "text": "string",
                      "options": [
                        {"text": "string", "isCorrect": true},
                        {"text": "string", "isCorrect": false},
                        {"text": "string", "isCorrect": false}
                      ]
                    }
                  }
                },
                {
                  "type": "test",
                  "title": "string",
                  "points": "number | null",
                  "content": {
                    "questions": [
                      {
                        "type": "singleChoice",
                        "text": "string",
                        "options": [
                          {"text": "string", "isCorrect": true},
                          {"text": "string", "isCorrect": false},
                          {"text": "string", "isCorrect": false}
                        ]
                      },
                      {
                        "type": "multipleChoice",
                        "text": "string",
                        "options": [
                          {"text": "string", "isCorrect": true},
                          {"text": "string", "isCorrect": true},
                          {"text": "string", "isCorrect": false}
                        ]
                      },
                      {
                        "type": "singleChoice",
                        "text": "string",
                        "options": [
                          {"text": "string", "isCorrect": true},
                          {"text": "string", "isCorrect": false}
                        ]
                      }
                    ]
                  }
                },
                {
                  "type": "checklist",
                  "title": "string",
                  "points": "number | null",
                  "content": ["string", "string"]
                },
                {
                  "type": "task",
                  "title": "string",
                  "points": "number | null",
                  "content": {
                    "text": "string"
                  }
                }
              ]
            }
          ],
          "aiComment": "string"
        }
        """;

    public (string systemPrompt, string userMessage) Build(ScenarioGenerationRequest request)
    {
        var systemPrompt = BuildSystemPrompt(request);
        var userMessage = BuildUserMessage(request);
        return (systemPrompt, userMessage);
    }

    private static string BuildSystemPrompt(ScenarioGenerationRequest request)
    {
        var sb = new StringBuilder(SystemPromptBase);

        sb.AppendLine();
        sb.AppendLine($"СТИЛЬ ТЕКСТА ДЛЯ ДАННОЙ ГЕНЕРАЦИИ: {GetStyleDescription(request.TextStyle)}");

        if (request.TotalPoints.HasValue)
        {
            sb.AppendLine();
            sb.AppendLine($"РАСПРЕДЕЛЕНИЕ БАЛЛОВ: Общее количество баллов за прохождение онбординга — {request.TotalPoints.Value}.");
            sb.AppendLine("Распредели их между шагами пропорционально их сложности и важности. Сумма всех полей \"points\" должна быть примерно равна указанному числу.");
            sb.AppendLine("Шаги типа \"information\" могут иметь points: null (информационные, без оценки), если иное не целесообразно.");
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine("БАЛЛЫ: Не задавай значения points (используй null для всех шагов).");
        }

        if (request.IncludeFinalTest)
        {
            sb.AppendLine();
            sb.AppendLine("ТЕСТ В КОНЦЕ ЭТАПА: В конце каждого этапа добавляй шаг типа \"test\" для проверки усвоенного материала.");
        }

        sb.AppendLine();
        sb.AppendLine("JSON-СХЕМА ОТВЕТА:");
        sb.AppendLine(ResponseSchema);

        return sb.ToString();
    }

    private string BuildUserMessage(ScenarioGenerationRequest request)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Запрос: {request.Query}");
        sb.AppendLine($"Рекомендуемое количество этапов: {request.StageCount}. Если материала недостаточно, можешь уменьшить это число, но старайся соответствовать данному значению");
        sb.AppendLine($"Рекомендуемое количество шагов в каждом этапе: {request.StepsPerStage}. Если материала недостаточно, можешь уменьшить это число, но старайся соответствовать данному значению");

        if (!string.IsNullOrWhiteSpace(request.TargetAudience))
            sb.AppendLine($"Целевая аудитория: {request.TargetAudience}");

        if (!string.IsNullOrWhiteSpace(request.AdditionalInfo))
            sb.AppendLine($"Дополнительные сведения: {request.AdditionalInfo}");

        if (request.Documents is { Count: > 0 })
        {
            var extracted = request.Documents
                .Select(doc => (doc.FileName, Text: _extractor.Extract(doc)))
                .Where(x => !string.IsNullOrWhiteSpace(x.Text))
                .ToList();

            if (extracted.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Используй содержимое приложенных документов как основной источник знаний:");
                foreach (var (fileName, text) in extracted)
                {
                    sb.AppendLine();
                    sb.AppendLine($"=== {fileName} ===");
                    sb.AppendLine(text);
                    sb.AppendLine($"=== конец {fileName} ===");
                }
            }
        }

        return sb.ToString();
    }

    private static string GetStyleDescription(string textStyle) => textStyle switch
    {
        "friendly" => "дружелюбный, неформальный («ты»), тёплый и поддерживающий",
        "strict" => "официальный, деловой («вы»), точный и структурированный",
        "simple" => "простой, доступный, короткие предложения, минимум профессионального жаргона",
        _ => "нейтральный"
    };
}
