using Microsoft.Extensions.Logging;
using YoutubeDownload.Infrastructure.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Infrastructure.Services
{
    public class YoutubeDownloadClient(YoutubeClient client, IFfmpegService ffmpegService, ILogger<YoutubeDownloadClient> logger) : IYoutubeDownloadClient
    {
        public async Task<Video> GetVideoAsync(string url, CancellationToken token = default)
        {
            logger.LogInformation("Starting manifest download for video [{Url}].", url);
            return await client.Videos.GetAsync(url, token);
        }

        public async Task<StreamManifest> GetManifestAsync(string videoId, CancellationToken token = default)
        {
            logger.LogInformation("Manifest successfully downloaded for video [{videoId}].", videoId);
            return await client.Videos.Streams.GetManifestAsync(videoId, token);
        }

        public async Task DownloaAudioAsync(IStreamInfo streamInfo, string filePath, CancellationToken token = default)
        {
            await client.Videos.Streams.DownloadAsync(streamInfo, filePath, null, token);
        }

        public async Task DownloadVideoAsync(IStreamInfo audioStreamInfo, IStreamInfo videoStreamInfo, string filePath, CancellationToken token = default)
        {
            var streams = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
            var conversionRequest = new ConversionRequestBuilder(filePath).SetFFmpegPath(ffmpegService.Path).Build();
            await client.Videos.DownloadAsync(streams, conversionRequest, null, token);
        }
    }
}
