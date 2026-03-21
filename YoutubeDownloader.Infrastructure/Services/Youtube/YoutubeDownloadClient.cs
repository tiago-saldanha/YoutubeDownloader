using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using YoutubeDownloader.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownloader.Infrastructure.Interfaces.Youtube;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Infrastructure.Services.Youtube
{
    public class YoutubeDownloadClient(
        YoutubeClient client, 
        IFfmpegService ffmpegService, 
        ILogger<YoutubeDownloadClient> logger) : IYoutubeDownloadClient
    {
        private readonly ConcurrentDictionary<string, Task<StreamManifest>> _manifestCache = new();

        public async Task<Video> GetVideoAsync(string url, CancellationToken token = default)
        {
            logger.LogInformation("Starting manifest download for video [{Url}].", url);
            return await client.Videos.GetAsync(url, token);
        }

        public async Task<StreamManifest> GetManifestAsync(string videoId, CancellationToken token = default)
            => await _manifestCache.GetOrAdd(videoId, async (id) => await FetchManifestInternal(id, token));

        public async Task DownloaAudioAsync(IStreamInfo streamInfo, string filePath, IProgress<double> progress, CancellationToken token = default)
        {
            await client.Videos.Streams.DownloadAsync(streamInfo, filePath, progress, token);
            logger.LogInformation("Audio download completed successfully. File saved at {FilePath}.", filePath);
        }

        public async Task DownloadVideoAsync(IStreamInfo audioStreamInfo, IStreamInfo videoStreamInfo, string filePath, IProgress<double> progress, CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Preparing download for video '{Url}'. Output file: {FilePath}.", videoStreamInfo.Url, filePath);
                var streams = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                var conversionRequest = new ConversionRequestBuilder(filePath).SetFFmpegPath(ffmpegService.Path).Build();
                await client.Videos.DownloadAsync(streams, conversionRequest, progress, token);
                logger.LogInformation("Video download completed successfully. File saved at {FilePath}.", filePath);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while downloading video from '{Url}'. Output file: {FilePath}.",
                    videoStreamInfo.Url,
                    filePath
                );

                throw;
            }
        }

        private async Task<StreamManifest> FetchManifestInternal(string videoId, CancellationToken token = default)
        {
            var manifest = await client.Videos.Streams.GetManifestAsync(videoId, token);
            logger.LogInformation("Manifest successfully downloaded for video [{videoId}].", videoId);
            return manifest;
        }
    }
}
