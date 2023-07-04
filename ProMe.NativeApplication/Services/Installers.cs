using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
