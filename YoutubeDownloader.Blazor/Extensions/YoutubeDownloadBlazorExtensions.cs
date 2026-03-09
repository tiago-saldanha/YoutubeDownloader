using YoutubeDownloader.Blazor.Http;
using YoutubeDownloader.Core.Interfaces;
using YoutubeDownloader.Core.Services;
using YoutubeDownloader.Domain.Interfaces;
using YoutubeDownloader.Infrastructure.Configuration;
using YoutubeDownloader.Infrastructure.Interfaces.Cache;
using YoutubeDownloader.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownloader.Infrastructure.Interfaces.Youtube;
using YoutubeDownloader.Infrastructure.Services.Cache;
using YoutubeDownloader.Infrastructure.Services.Cleanner;
using YoutubeDownloader.Infrastructure.Services.Ffmpeg;
using YoutubeDownloader.Infrastructure.Services.Youtube;
using YoutubeExplode;

namespace YoutubeDownloader.Blazor.Extensions
{
    public static class YoutubeDownloadBlazorExtensions
    {
        public static WebApplicationBuilder ConfigureHttpClient(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient<DownloadApiClient>(options =>
            {
                options.BaseAddress = new Uri(
                    builder.Configuration["App:BaseUrl"]!
                );
            });
            return builder;
        }

        public static WebApplicationBuilder ConfigureWeb(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
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
            builder.Services.AddSingleton<YoutubeClient>();
            builder.Services.AddScoped<IYoutubeDownloadClient, YoutubeDownloadClient>();
            builder.Services.AddScoped<IYoutubeAppService, YoutubeAppService>();
            builder.Services.AddScoped<IYoutubeService, YoutubeService>();
            builder.Services.AddSingleton<IFfmpegService, FfmpegService>();
            return builder;
        }

        public static WebApplicationBuilder ConfigureInfraStructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<CleannerService>();
            builder.Services.Configure<FfmpegOptions>(builder.Configuration.GetSection("Ffmpeg"));
            return builder;
        }
    }
}
