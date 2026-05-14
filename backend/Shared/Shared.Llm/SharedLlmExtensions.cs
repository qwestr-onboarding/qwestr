using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Shared.Llm.Clients;
using Qwestr.Shared.Llm.Options;

namespace Qwestr.Shared.Llm;

public static class SharedLlmExtensions
{
    public static IServiceCollection AddSharedLlm(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LlmSharedOptions>(configuration.GetSection(LlmSharedOptions.SectionName));

        services.AddHttpClient<YandexGptGenericClient>();
        services.AddScoped<GeminiGenericClient>();
        services.AddHttpClient<QwenGenericClient>();
        services.AddSingleton<FakeGenericLlmClient>();

        services.AddScoped<IGenericLlmClientFactory, GenericLlmClientFactory>();

        return services;
    }
}
