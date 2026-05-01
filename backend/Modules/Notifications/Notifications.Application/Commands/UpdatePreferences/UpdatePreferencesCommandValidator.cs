using FluentValidation;

namespace Qwestr.Modules.Notifications.Application.Commands.UpdatePreferences;

public sealed class UpdatePreferencesCommandValidator : AbstractValidator<UpdatePreferencesCommand>
{
    public UpdatePreferencesCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Preferences).NotEmpty();
    }
}
