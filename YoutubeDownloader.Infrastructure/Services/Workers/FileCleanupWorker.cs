using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using YoutubeDownloader.Infrastructure.Helpers;

namespace YoutubeDownloader.Infrastructure.Services.Workers
{
    public class FileCleanupWorker(
        Channel<string> channel,
        ILogger<FileCleanupWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("FileCleanupWorker started.");

            var channelTask = ProcessChannelAsync(stoppingToken);
            var ttlTask = ProcessTtlCleanupAsync(stoppingToken);

            await Task.WhenAll(channelTask, ttlTask);

            logger.LogInformation("CleanerService stopped.");
        }

        private async Task ProcessChannelAsync(CancellationToken token)
        {
            await foreach (var filePath in channel.Reader.ReadAllAsync(token))
            {
                try
                {
                    await FileSystemManager.TryDeleteAsync(filePath);

                    logger.LogInformation(
                        "Deleted file via channel: {FilePath}",
                        filePath);
                }
                catch (IOException ex)
                {
                    logger.LogWarning(
                        ex,
                        "File in use, retry later: {FilePath}",
                        filePath);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Error deleting file from channel: {FilePath}",
                        filePath);
                }
            }
        }

        private async Task ProcessTtlCleanupAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var directory = new DirectoryInfo(FileSystemManager.OutputDirectory);

                    if (!directory.Exists)
                    {
                        logger.LogWarning("Output directory does not exist.");
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                        continue;
                    }

                    var expirationTime = DateTime.UtcNow.AddMinutes(-15);

                    foreach (var file in directory.GetFiles())
                    {
                        if (file.LastWriteTimeUtc < expirationTime)
                        {
                            file.Delete();

                            logger.LogInformation(
                                "Deleted expired file: {FilePath}",
                                file.FullName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error in CleanerService");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
