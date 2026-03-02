using YoutubeExplode;
using YoutubeDownload.Core.Interfaces;
using YoutubeDownload.Core.Services;
using YoutubeDownload.Infrastructure.Configuration;
using YoutubeDownload.Domain.Interfaces;
using YoutubeDownload.Infrastructure.Services.Cache;
using YoutubeDownload.Infrastructure.Services.Ffmpeg;
using YoutubeDownload.Infrastructure.Services.Youtube;
using YoutubeDownload.Infrastructure.Interfaces.Cache;
using YoutubeDownload.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownload.Infrastructure.Interfaces.Youtube;
using Microsoft.Extensions.Configuration;
using YoutubeDownload.Maui.Services;

namespace YoutubeDownload.Maui.Extensions
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
