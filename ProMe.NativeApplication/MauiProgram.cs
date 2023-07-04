using Microsoft.Extensions.Logging;
using ProMe.NativeApplication.Data;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using ProMe.NativeApplication.Services;
using System.Security.Claims;

namespace ProMe.NativeApplication;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<WeatherForecastService>();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddTools();
        builder.Services.AddScoped<IdentityAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<IdentityAuthenticationStateProvider>());

        return builder.Build();
    }
}
