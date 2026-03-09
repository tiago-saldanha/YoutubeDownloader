using YoutubeExplode;
using YoutubeDownloader.Core.Interfaces;
using YoutubeDownloader.Core.Services;
using YoutubeDownloader.Infrastructure.Configuration;
using YoutubeDownloader.Domain.Interfaces;
using YoutubeDownloader.Infrastructure.Services.Cache;
using YoutubeDownloader.Infrastructure.Services.Ffmpeg;
using YoutubeDownloader.Infrastructure.Services.Youtube;
using YoutubeDownloader.Infrastructure.Interfaces.Cache;
using YoutubeDownloader.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownloader.Infrastructure.Interfaces.Youtube;
using Microsoft.Extensions.Configuration;
using YoutubeDownloader.Maui.Services;

namespace YoutubeDownloader.Maui.Extensions
{
    public static class YoutubeDownloadMauiExtensions
    {
        public static MauiAppBuilder ConfigureCache(this MauiAppBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IStorageCacheService, StorageCacheService>();
            return builder;
        }

        public static MauiAppBuilder ConfigureApplication(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<YoutubeClient>();
            builder.Services.AddScoped<IYoutubeDownloadClient, YoutubeDownloadClient>();
            builder.Services.AddScoped<IYoutubeAppService, YoutubeAppService>();
            builder.Services.AddScoped<IYoutubeService, YoutubeService>();
            builder.Services.AddSingleton<IFfmpegService, FfmpegService>();
            builder.Services.AddSingleton<IFileSaveService, MauiFileSaveService>();
            return builder;
        }

        public static MauiAppBuilder ConfigureInfraStructure(this MauiAppBuilder builder)
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;

            builder.Configuration.AddJsonStream(stream);

            builder.Services.Configure<FfmpegOptions>(options =>
            {
                options.Path = builder.Configuration["Ffmpeg:Path"];
            });

            return builder;
        }   
    }
}
