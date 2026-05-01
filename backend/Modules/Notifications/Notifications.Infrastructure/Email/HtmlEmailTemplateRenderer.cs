using System.Reflection;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;

namespace Qwestr.Modules.Notifications.Infrastructure.Email;

public sealed class HtmlEmailTemplateRenderer : IEmailTemplateRenderer
{
    private static readonly Dictionary<string, string> Subjects = new()
    {
        ["Invitation"] = "Вас пригласили в {CompanyName}",
        ["Welcome"] = "Добро пожаловать в {CompanyName}!",
        ["ScenarioAssigned"] = "Вам назначен сценарий: {ScenarioName}",
        ["ScenarioCompleted"] = "Сценарий адаптации завершён!",
        ["BadgeEarned"] = "Вы получили награду: {BadgeName}",
        ["LevelUp"] = "Поздравляем! Новый уровень {NewLevel}!",
    };

    private static readonly string LayoutTemplate =
        "<!DOCTYPE html>" +
        "<html lang=\"ru\">" +
        "<head>" +
        "<meta charset=\"utf-8\">" +
        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" +
        "<style>" +
        "body{font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;margin:0;padding:0;background:#f5f5f5;color:#333}" +
        ".container{max-width:600px;margin:40px auto;background:#fff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,.1)}" +
        ".header{background:#1976D2;padding:24px 32px}" +
        ".header h1{color:#fff;margin:0;font-size:22px;font-weight:600}" +
        ".body{padding:32px;line-height:1.6}" +
        ".body p{margin:0 0 16px}" +
        ".cta{display:inline-block;background:#1976D2;color:#fff;padding:12px 24px;border-radius:6px;text-decoration:none;font-weight:600}" +
        ".footer{padding:20px 32px;background:#f9f9f9;border-top:1px solid #eee;font-size:12px;color:#999}" +
        "</style>" +
        "</head>" +
        "<body>" +
        "<div class=\"container\">" +
        "<div class=\"header\"><h1>Qwestr</h1></div>" +
        "<div class=\"body\">{CONTENT}</div>" +
        "<div class=\"footer\">Это письмо отправлено автоматически системой Qwestr. Не отвечайте на него.</div>" +
        "</div>" +
        "</body>" +
        "</html>";

    public Task<RenderedEmail> RenderAsync(string templateName, object model, CancellationToken cancellationToken = default)
    {
        var props = model.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, p => p.GetValue(model)?.ToString() ?? string.Empty);

        var subject = Subjects.TryGetValue(templateName, out var subjectTemplate)
            ? ApplyPlaceholders(subjectTemplate, props)
            : templateName;

        var bodyContent = GetBodyContent(templateName, props);
        var bodyHtml = LayoutTemplate.Replace("{CONTENT}", bodyContent);

        return Task.FromResult(new RenderedEmail(subject, bodyHtml, null));
    }

    private static string GetBodyContent(string templateName, Dictionary<string, string> props)
    {
        var v = props;
        return templateName switch
        {
            "Invitation" =>
                "<p>Здравствуйте!</p>" +
                "<p><strong>" + v.GetValueOrDefault("InviterName") + "</strong> приглашает вас присоединиться к " +
                "<strong>" + v.GetValueOrDefault("CompanyName") + "</strong> на платформе Qwestr.</p>" +
                "<p>Нажмите кнопку ниже, чтобы принять приглашение:</p>" +
                "<p><a href=\"" + v.GetValueOrDefault("ActivationUrl") + "\" class=\"cta\">Принять приглашение</a></p>" +
                "<p>Ссылка действительна 7 дней.</p>",

            "Welcome" =>
                "<p>Привет, <strong>" + v.GetValueOrDefault("UserName") + "</strong>!</p>" +
                "<p>Добро пожаловать в <strong>" + v.GetValueOrDefault("CompanyName") + "</strong>. Ваш аккаунт активирован.</p>" +
                "<p><a href=\"" + v.GetValueOrDefault("AppUrl") + "\" class=\"cta\">Перейти в Qwestr</a></p>",

            "ScenarioAssigned" =>
                "<p>Привет, <strong>" + v.GetValueOrDefault("UserName") + "</strong>!</p>" +
                "<p>Вам назначен новый сценарий адаптации: <strong>" + v.GetValueOrDefault("ScenarioName") + "</strong>.</p>" +
                "<p><a href=\"" + v.GetValueOrDefault("ScenarioUrl") + "\" class=\"cta\">Начать сценарий</a></p>",

            "ScenarioCompleted" =>
                "<p>Отличная работа, <strong>" + v.GetValueOrDefault("UserName") + "</strong>!</p>" +
                "<p>Вы успешно завершили сценарий адаптации: <strong>" + v.GetValueOrDefault("ScenarioName") + "</strong>.</p>" +
                "<p>Вы заработали <strong>" + v.GetValueOrDefault("TotalPoints") + "</strong> баллов.</p>",

            "BadgeEarned" =>
                "<p>Поздравляем, <strong>" + v.GetValueOrDefault("UserName") + "</strong>!</p>" +
                "<p>Вы получили новую награду: <strong>" + v.GetValueOrDefault("BadgeName") + "</strong>.</p>" +
                (string.IsNullOrEmpty(v.GetValueOrDefault("BadgeDescription")) ? "" :
                    "<p>" + v.GetValueOrDefault("BadgeDescription") + "</p>") +
                "<p><a href=\"" + v.GetValueOrDefault("ProfileUrl") + "\" class=\"cta\">Посмотреть профиль</a></p>",

            "LevelUp" =>
                "<p>Поздравляем, <strong>" + v.GetValueOrDefault("UserName") + "</strong>!</p>" +
                "<p>Вы достигли нового уровня: <strong>" + v.GetValueOrDefault("NewLevel") + "</strong>!</p>" +
                "<p><a href=\"" + v.GetValueOrDefault("ProfileUrl") + "\" class=\"cta\">Посмотреть профиль</a></p>",

            _ => "<p>У вас новое уведомление из Qwestr.</p>"
        };
    }

    private static string ApplyPlaceholders(string template, Dictionary<string, string> props)
    {
        foreach (var (key, value) in props)
        {
            template = template.Replace("{" + key + "}", value);
        }
        return template;
    }
}
