using Client.Infrastructure.Api;
using Client.Infrastructure.Api.Endpoints;
using Client.Infrastructure.Auth;
using Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Client.Infrastructure;

public static class InfrastructureStartup
{
    public static IServiceCollection AddQwestrApi(this IServiceCollection services, string baseUrl)
    {
        services.AddScoped<TokenStorageService>();
        services.AddScoped<AuthHeaderHandler>();

        services.AddHttpClient<QwestrApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

        // --- Identity ---
        services.AddScoped<AuthApi>();
        services.AddScoped<UsersApi>();
        services.AddScoped<InvitationsApi>();
        services.AddScoped<TenantApi>();

        services.AddHttpContextAccessor();
        services.AddScoped<TenantResolver>();
        services.AddScoped<BffAuthClient>();
        services.AddScoped<SessionManager>();

        services.AddScoped<JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<JwtAuthStateProvider>());

        return services;
    }
}