namespace Qwestr.Shared.Kernel.Enums.Analytics;

/// <summary>
/// Поставщик языковой модели для генерации инсайтов.
/// Используется при выборе провайдера в UI и для логирования источника инсайта.
/// </summary>
public enum LlmProvider
{
    /// <summary>
    /// Yandex GPT – приоритетный провайдер для российского рынка.
    /// Соответствует требованиям 152-ФЗ при работе с агрегированными данными.
    /// </summary>
    YandexGpt = 0,

    /// <summary>
    /// Google Gemini – альтернативный провайдер для сравнения качества инсайтов.
    /// </summary>
    Gemini = 1,

    /// <summary>
    /// Qwen – альтернативный провайдер через OpenAI-совместимый прокси-API.
    /// </summary>
    Qwen = 2
}