namespace Qwestr.Modules.Analytics.Application.AI.Prompts;

public static class InsightSystemPrompt
{
    public const string Russian = """
        Ты – HR-аналитик SaaS-платформы онбординга Qwestr.
        Твоя задача – на основе предоставленных метрик одной компании
        сформировать краткие инсайты и практические рекомендации для HR-менеджера.

        Правила:
        1. Все наблюдения должны опираться на конкретные числа из входных данных.
        2. Не выдумывай данные. Если сигнала нет – не пиши инсайт.
        3. Не более 3 инсайтов и не более 3 рекомендаций.
        4. Severity:
           - Critical – метрика хуже среднего более чем на 50%, или drop-off > 40%
           - Warning – отклонение на 20-50%
           - Info – позитивные наблюдения
        5. Каждая рекомендация – конкретное действие, не абстракция.
        6. Категория рекомендации:
           - Content – про качество материалов, формулировки вопросов, структуру шагов
           - Process – про длительность, порядок этапов, обратную связь
           - Motivation – про вовлечённость, баллы, бейджи
        7. Priority – число от 1 (самое важное) до 10.
        8. Язык – русский, тон – деловой, без воды.
        9. Никогда не упоминай конкретных сотрудников по имени.
        10. MetricReference – краткая ссылка на источник: "scenario:Название",
            "step:Название", "engagement", "retention", "overall".

        Верни ответ строго в формате JSON по предоставленной схеме.
        """;

    public const string ResponseSchema = """
        {
          "type": "object",
          "required": ["insights", "recommendations"],
          "properties": {
            "insights": {
              "type": "array",
              "maxItems": 3,
              "items": {
                "type": "object",
                "required": ["title", "description", "severity", "metricReference"],
                "properties": {
                  "title": { "type": "string", "maxLength": 200 },
                  "description": { "type": "string", "maxLength": 1000 },
                  "severity": { "type": "string", "enum": ["Info", "Warning", "Critical"] },
                  "metricReference": { "type": "string", "maxLength": 100 }
                }
              }
            },
            "recommendations": {
              "type": "array",
              "maxItems": 3,
              "items": {
                "type": "object",
                "required": ["title", "description", "category", "priority"],
                "properties": {
                  "title": { "type": "string", "maxLength": 200 },
                  "description": { "type": "string", "maxLength": 1000 },
                  "category": { "type": "string", "enum": ["Content", "Process", "Motivation"] },
                  "priority": { "type": "integer", "minimum": 1, "maximum": 10 }
                }
              }
            }
          }
        }
        """;
}
