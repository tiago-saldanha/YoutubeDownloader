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
using YoutubeDownloader.SharedUI.Services;
using YoutubeExplode;

namespace YoutubeDownloader.Desktop.Extensions
{
    public static class YoutubeDownloaderDesktopExtensions
    {
        public static MauiAppBuilder ConfigureHttpClient(this MauiAppBuilder builder)
        {
            var baseUrl = builder.Configuration["App:BaseUrl"];

            builder.Services.AddHttpClient<DownloadApiClient>(options =>
            {
                options.BaseAddress = new Uri(baseUrl!);
            });
            return builder;
        }

        public static MauiAppBuilder ConfigureCache(this MauiAppBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IStorageCacheService, StorageCacheService>();
            builder.Services.AddSingleton<IDeviceService, MauiDeviceService>();
            return builder;
        }

        public static MauiAppBuilder ConfigureApplication(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<YoutubeClient>();
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