using Microsoft.Extensions.Logging;
using YoutubeDownload.Application.Exceptions;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Domain.Interfaces;
using YoutubeDownload.Domain.ViewModel;

namespace YoutubeDownload.Application.Services
{
    public class YoutubeAppService(IYoutubeService youtubeService, ILogger<YoutubeAppService> logger) 
        : IYoutubeAppService
    {
        public async Task<StreamManifestViewModel> DownloadManifestAsync(string url)
        {
            try
            {
                var manifest = await youtubeService.DownloadManifestAsync(url);
                return manifest;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while downloading manifest for video [{Url}].", url);
                throw new VideoManifestDownloadException(url, ex);
            }
        }

        public async Task<DownloadStreamViewModel> DownloadAsync(DownloadCommand command)
        {
            try
            {
                logger.LogInformation("Starting video download [{Title}] (ID: {VideoId}).", command.Title, command.VideoId);
                return await youtubeService.DownloadStreamAsync(command);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while downloading video [{Title}] (ID: {VideoId}).", command.Title, command.VideoId);

                throw new VideoDownloadException(command.VideoId, command.Title, ex);
            }
        }
    }
}
