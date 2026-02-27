using YoutubeExplode;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Application.Services;
using YoutubeDownload.Infrastructure.Configuration;
using YoutubeDownload.Infrastructure.Interfaces;
using YoutubeDownload.Infrastructure.Services;
using YoutubeDownload.Blazor.Cache;
using YoutubeDownload.Blazor.Http;
using YoutubeDownload.Blazor.Interfaces;

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
            builder.Services.AddScoped<IStorageCache, StorageCache>();
            return builder;
        }

        public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<YoutubeClient>();
            builder.Services.AddScoped<IYoutubeAppService, YoutubeAppService>();
            builder.Services.AddScoped<IYoutubeService, YoutubeService>();
            builder.Services.AddSingleton<IFfmpegService, FfmpegService>();
            return builder;
        }

        public static WebApplicationBuilder ConfigureInfraStructure(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<FfmpegOptions>(builder.Configuration.GetSection("Ffmpeg"));
            return builder;
        }
    }
}
