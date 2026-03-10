using FluentValidation;
using Identity.Api.Endpoints;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Services;
using Identity.Application.Behaviors;
using Identity.Application.Commands.Login;
using Identity.Infrastructure;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api;

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
