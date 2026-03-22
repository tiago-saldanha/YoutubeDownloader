using Microsoft.Extensions.Configuration;
using MudBlazor;
using MudBlazor.Services;
using System.Reflection;
using YoutubeDownloader.Core.Interfaces;
using YoutubeDownloader.Core.Services;
using YoutubeDownloader.Desktop.Services;
using YoutubeDownloader.Domain.Interfaces;
using YoutubeDownloader.Infrastructure.Configuration;
using YoutubeDownloader.Infrastructure.Interfaces.Cache;
using YoutubeDownloader.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownloader.Infrastructure.Interfaces.Youtube;
using YoutubeDownloader.Infrastructure.Services.Cache;
using YoutubeDownloader.Infrastructure.Services.Ffmpeg;
using YoutubeDownloader.Infrastructure.Services.Youtube;
using YoutubeDownloader.SharedUI.Interfaces;

namespace YoutubeDownloader.Desktop.Extensions
{
    public static class YoutubeDownloaderDesktopExtensions
    {
        public static MauiAppBuilder ConfigureAppSettings(this MauiAppBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("YoutubeDownloader.Desktop.appsettings.json");

            if (stream != null)
            {
                builder.Configuration.AddJsonStream(stream);
            }

            return builder;
        }

        public static MauiAppBuilder ConfigureMaui(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IDeviceService, MauiDeviceService>();
            builder.Services.AddSingleton<IAppInfoService, MauiAppInfoService>();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                config.SnackbarConfiguration.PreventDuplicates = true;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 4000;
            });
            return builder;
        }

        public static MauiAppBuilder ConfigureCache(this MauiAppBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IStorageCacheService, StorageCacheService>();
            
            return builder;
        }

        public static MauiAppBuilder ConfigureApplication(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IYoutubeDownloadClient, YoutubeDownloadClient>();
            builder.Services.AddSingleton<IYoutubeAppService, YoutubeAppService>();
            builder.Services.AddSingleton<IYoutubeService, YoutubeService>();
            builder.Services.AddSingleton<IFfmpegService, FfmpegService>();
            return builder;
        }

        public static MauiAppBuilder ConfigureInfrastructure(this MauiAppBuilder builder)
        {
            builder.Services.Configure<FfmpegOptions>(builder.Configuration.GetSection("Ffmpeg"));
            return builder;
        }
    }
}