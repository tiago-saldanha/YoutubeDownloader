using YoutubeDownload.Blazor.Http;
using YoutubeDownload.Core.Interfaces;
using YoutubeDownload.Core.Services;
using YoutubeDownload.Domain.Interfaces;
using YoutubeDownload.Infrastructure.Configuration;
using YoutubeDownload.Infrastructure.Interfaces.Cache;
using YoutubeDownload.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownload.Infrastructure.Interfaces.Youtube;
using YoutubeDownload.Infrastructure.Services;
using YoutubeDownload.Infrastructure.Services.Cache;
using YoutubeDownload.Infrastructure.Services.Ffmpeg;
using YoutubeDownload.Infrastructure.Services.Youtube;
using YoutubeExplode;

namespace YoutubeDownload.Blazor.Extensions
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
