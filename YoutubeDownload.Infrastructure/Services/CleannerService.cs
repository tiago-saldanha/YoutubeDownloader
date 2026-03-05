using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YoutubeDownload.Infrastructure.Helpers;

namespace YoutubeDownload.Infrastructure.Services
{
    public class CleannerService(ILogger<CleannerService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("CleanerService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var directory = new DirectoryInfo(FileSystemManager.OutputDirectory);

                    if (!directory.Exists)
                    {
                        logger.LogWarning("Output director dos not exist.");
                        continue;
                    }

                    var expirationTime = DateTime.UtcNow.AddMinutes(-15);

                    foreach (var file in directory.GetFiles())
                    {
                        try
                        {
                            if (file.LastWriteTimeUtc < expirationTime)
                            {
                                file.Delete();

                                logger.LogInformation(
                                    "Deleted expired file: {Filepath}",
                                    file.FullName);
                            }
                        }
                        catch (IOException ex)
                        {
                            logger.LogError(
                                ex,
                                "File in use, could not delete: {FilePath}",
                                file.FullName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error in CleannerService");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            logger.LogInformation("CleanerService stopped.");
        }
    }
}
