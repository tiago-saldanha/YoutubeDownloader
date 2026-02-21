using Microsoft.Extensions.DependencyInjection;
using WindowsApp;
using Serilog;
using YoutubeDownload.CrossCutting.Extensions;

namespace YoutubeDownload.App.Factory
{
    public static class FactoryApp
    {
        public static FormApp Build() 
            => new ServiceCollection().ConfigureLogging().ProvideServices().Start();

        private static IServiceCollection ConfigureLogging(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs\\youtube-download.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddLogging(configure => configure.AddSerilog(dispose: true));
            return services;
        }

        private static FormApp Start(this IServiceCollection services) 
            => services.AddScoped<FormApp>().BuildServiceProvider().GetRequiredService<FormApp>();
    }
}
