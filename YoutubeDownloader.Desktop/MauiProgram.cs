using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using YoutubeDownloader.Desktop.Extensions;

namespace YoutubeDownloader.Desktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .ConfigureAppSettings()
                .ConfigureMaui()
                .ConfigureCache()
                .ConfigureApplication()
                .ConfigureInfrastructure();

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

            return builder.Build();
        }
    }
}
