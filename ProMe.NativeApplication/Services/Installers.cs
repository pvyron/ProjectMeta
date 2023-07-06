using System.Text.Json;

namespace ProMe.NativeApplication.Services;
public static class Installers
{
    public static IServiceCollection AddTools(this IServiceCollection services)
    {
        services.AddSingleton(new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return services;
    }
}
