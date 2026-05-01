using Qwestr.Modules.Notifications.Application.Models;

namespace Qwestr.Modules.Notifications.Application.Abstractions;

public interface IEmailTemplateRenderer
{
    Task<RenderedEmail> RenderAsync(string templateName, object model, CancellationToken cancellationToken = default);
}
