using YoutubeExplode;
using YoutubeDownload.Core.Interfaces;
using YoutubeDownload.Core.Services;
using YoutubeDownload.Infrastructure.Configuration;
using YoutubeDownload.Domain.Interfaces;
using YoutubeDownload.Infrastructure.Services.Ffmpeg;
using YoutubeDownload.Infrastructure.Services.Youtube;
using YoutubeDownload.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownload.Infrastructure.Interfaces.Youtube;
using YoutubeDownload.Infrastructure.Interfaces.Cache;
using YoutubeDownload.Infrastructure.Services.Cache;

namespace YoutubeDownload.Web.Extensions
{
    public static class YoutubeDownloadWebExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services) 
            => services.ConfigureYoutubeClient();

        private static IServiceCollection ConfigureYoutubeClient(this IServiceCollection services)
        {
            services.AddSingleton<YoutubeClient>();
            services.AddScoped<IYoutubeDownloadClient, YoutubeDownloadClient>();
            services.AddScoped<IYoutubeAppService, YoutubeAppService>();
            services.AddScoped<IYoutubeService, YoutubeService>();
            services.AddSingleton<IFfmpegService, FfmpegService>();
            return services;
        }

        public static WebApplicationBuilder AddInfraStructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IStorageCacheService, StorageCacheService>();
            builder.Services.Configure<FfmpegOptions>(builder.Configuration.GetSection("Ffmpeg"));
            return builder;
        }
    }
}
