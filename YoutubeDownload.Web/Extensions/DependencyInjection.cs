using YoutubeExplode;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Application.Services;

namespace YoutubeDownload.Web.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection ProvideServices(this IServiceCollection services) => services.ConfigureYoutubeClient();

        private static IServiceCollection ConfigureYoutubeClient(this IServiceCollection services)
        {
            services.AddSingleton<YoutubeClient>();
            services.AddScoped<IYoutubeService, YoutubeService>();
            return services;
        }
    }
}
