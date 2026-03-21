using Microsoft.Extensions.Logging;
using YoutubeDownloader.Core.Exceptions;
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
        public async Task<StreamManifestViewModel> DownloadManifestAsync(DownloadManifestCommand command)
        {
            try
            {
                return await youtubeService.DownloadManifestAsync(command);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while downloading manifest for video [{Url}].", command.Url);
                throw new DownloadManifestAppException(command.Url, ex);
            }
        }

        public async Task<DownloadFileViewModel> DownloadFileAsync(DownloadCommand command, IProgress<double> progress, CancellationToken token = default)
        {
            logger.LogInformation("Starting video download [{Title}] (ID: {VideoId}).", command.Title, command.VideoId);
            return await youtubeService.DownloadFileAsync(command, progress, token);
        }
    }
}
