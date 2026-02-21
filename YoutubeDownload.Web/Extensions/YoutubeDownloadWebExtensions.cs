using YoutubeExplode;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Application.Services;
using YoutubeDownload.Infrastructure.Configuration;
using YoutubeDownload.Infrastructure.Interfaces;
using YoutubeDownload.Infrastructure.Services;

namespace YoutubeDownload.Web.Extensions
{
    public static class YoutubeDownloadWebExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services) 
            => services.ConfigureYoutubeClient();

        private static IServiceCollection ConfigureYoutubeClient(this IServiceCollection services)
        {
            services.AddSingleton<YoutubeClient>();
            services.AddScoped<IYoutubeService, YoutubeService>();
            services.AddSingleton<IFfmpegService, FfmpegService>();
            return services;
        }

        public static WebApplicationBuilder AddInfraStructure(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<FfmpegOptions>(builder.Configuration.GetSection("Ffmpeg"));
            return builder;
        }
    }
}
