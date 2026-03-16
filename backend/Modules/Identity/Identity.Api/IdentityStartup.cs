using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Identity.Api.Endpoints;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Modules.Identity.Application.Behaviors;
using Qwestr.Modules.Identity.Application.Commands.Login;
using Qwestr.Modules.Identity.Infrastructure;
using Qwestr.Modules.Identity.Infrastructure.Persistence.Repositories;
using Qwestr.Modules.Identity.Infrastructure.Services;

namespace Qwestr.Modules.Identity.Api;

public static class IdentityStartup
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IdentityDbContext>(o =>
            o.UseNpgsql(
                config.GetConnectionString("Identity"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity"))
             .UseSnakeCaseNamingConvention());

        services.Configure<JwtSettings>(config.GetSection("Jwt"));

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ITotpService, TotpService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IUserProvider, UserProvider>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IEndpointRouteBuilder MapIdentityModule(this IEndpointRouteBuilder app)
    {
        app.MapIdentityEndpoints();
        return app;
    }
}
