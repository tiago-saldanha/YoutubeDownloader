using YoutubeExplode;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.ViewModel;
using Microsoft.Extensions.Logging;
using YoutubeDownload.Application.Exceptions;
using YoutubeDownload.Infrastructure.Interfaces;

namespace YoutubeDownload.Application.Services
{
    public class YoutubeAppService(YoutubeClient client, IYoutubeService youtubeService, IFfmpegService ffmpegService, ILogger<YoutubeAppService> logger) 
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

                var manifest = await client.Videos.Streams.GetManifestAsync(command.VideoId);

                return command.IsAudioOnly
                    ? await youtubeService.DownloadAudioAsync(manifest, command)
                    : await youtubeService.DownloadVideoAsync(manifest, command);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while downloading video [{Title}] (ID: {VideoId}).", command.Title, command.VideoId);

                throw new VideoDownloadException(command.VideoId, command.Title, ex);
            }
        }
    }
}
