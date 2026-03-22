using MudBlazor;
using MudBlazor.Services;
using System.Threading.Channels;
using YoutubeDownloader.Web.Services;
using YoutubeDownloader.Core.Interfaces;
using YoutubeDownloader.Core.Services;
using YoutubeDownloader.Domain.Interfaces;
using YoutubeDownloader.Infrastructure.Configuration;
using YoutubeDownloader.Infrastructure.Interfaces.Cache;
using YoutubeDownloader.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownloader.Infrastructure.Interfaces.Youtube;
using YoutubeDownloader.Infrastructure.Services.Cache;
using YoutubeDownloader.Infrastructure.Services.Ffmpeg;
using YoutubeDownloader.Infrastructure.Services.Workers;
using YoutubeDownloader.Infrastructure.Services.Youtube;
using YoutubeDownloader.SharedUI.Interfaces;

namespace YoutubeDownloader.Web.Extensions
{
    public static class YoutubeDownloaderWebExtensions
    {
        public static WebApplicationBuilder ConfigureWeb(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IDeviceService, WebDeviceService>();
            builder.Services.AddSingleton<IAppInfoService, WebAppInfoService>();
            builder.Services.AddSingleton<Channel<string>>(_ => Channel.CreateUnbounded<string>());
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

        public static WebApplicationBuilder ConfigureCache(this WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IStorageCacheService, StorageCacheService>();
            return builder;
        }

        public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IYoutubeDownloadClient, YoutubeDownloadClient>();
            builder.Services.AddScoped<IYoutubeAppService, YoutubeAppService>();
            builder.Services.AddScoped<IYoutubeService, YoutubeService>();
            builder.Services.AddSingleton<IFfmpegService, FfmpegService>();
            return builder;
        }

        public static WebApplicationBuilder ConfigureInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<FileCleanupWorker>();
            builder.Services.Configure<FfmpegOptions>(builder.Configuration.GetSection("Ffmpeg"));
            return builder;
        }
    }
}
