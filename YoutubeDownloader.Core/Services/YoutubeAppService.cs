using Microsoft.Extensions.Logging;
using YoutubeDownloader.Core.Interfaces;
using YoutubeDownloader.Domain.Commands;
using YoutubeDownloader.Domain.Interfaces;
using YoutubeDownloader.Domain.ViewModel;

namespace YoutubeDownloader.Core.Services
{
    public class YoutubeAppService(
        IYoutubeService youtubeService, 
        ILogger<YoutubeAppService> logger) : IYoutubeAppService
    {
        public async Task<StreamManifestViewModel> DownloadManifestAsync(
            DownloadManifestCommand command, 
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting manifest download for video [{Url}].", command.Url);
            return await youtubeService.DownloadManifestAsync(command, cancellationToken);
        }

        public async Task<DownloadFileViewModel> DownloadFileAsync(
            DownloadCommand command, 
            IProgress<double> progress, 
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Starting video download for [{Title}] (ID: {VideoId}).",
                command.Title,
                command.VideoId);
            return await youtubeService.DownloadFileAsync(command, progress, cancellationToken);
        }
    }
}
