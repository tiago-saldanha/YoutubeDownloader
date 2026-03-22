using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using YoutubeDownloader.Infrastructure.Interfaces.Ffpmeg;
using YoutubeDownloader.Infrastructure.Interfaces.Youtube;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Infrastructure.Services.Youtube
{
    public class YoutubeDownloadClient : IYoutubeDownloadClient
    {
        private readonly Lazy<Task<YoutubeClient>> _clientLazy;
        private readonly IFfmpegService _ffmpegService;
        private readonly ILogger<YoutubeDownloadClient> _logger;
        private readonly ConcurrentDictionary<string, Task<StreamManifest>> _manifestCache = new();

        public YoutubeDownloadClient(
            IFfmpegService ffmpegService,
            ILogger<YoutubeDownloadClient> logger)
        {
            _ffmpegService = ffmpegService;
            _logger = logger;
            _clientLazy = new Lazy<Task<YoutubeClient>>(CreateClientAsync);
        }

        private async Task<YoutubeClient> CreateClientAsync()
        {
            var cookieContainer = new CookieContainer();

            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            using var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/92.0.4515.107 Safari/537.36");

            var requestUri = new Uri("https://www.youtube.com");
            await httpClient.GetAsync(requestUri);

            var cookies = cookieContainer.GetCookies(requestUri).ToList().AsReadOnly();

            return new YoutubeClient(cookies);
        }

        private async Task<YoutubeClient> GetClientAsync()
            => await _clientLazy.Value;

        public async Task<Video> GetVideoAsync(string url, CancellationToken token = default)
        {
            var client = await GetClientAsync();
            _logger.LogInformation("Starting video fetch for [{Url}].", url);
            return await client.Videos.GetAsync(url, token);
        }

        public async Task<StreamManifest> GetManifestAsync(string videoId, CancellationToken token = default)
        {
            try
            {
                return await _manifestCache.GetOrAdd(videoId, _ => FetchManifestInternal(videoId, token));
            }
            catch
            {
                _manifestCache.TryRemove(videoId, out _);
                throw;
            }
        }

        public async Task DownloaAudioAsync(
            IStreamInfo streamInfo,
            string filePath,
            IProgress<double> progress,
            CancellationToken token = default)
        {
            var client = await GetClientAsync();
            await client.Videos.Streams.DownloadAsync(streamInfo, filePath, progress, token);
            _logger.LogInformation("Audio download completed successfully. File saved at {FilePath}.", filePath);
        }

        public async Task DownloadVideoAsync(
            IStreamInfo audioStreamInfo,
            IStreamInfo videoStreamInfo,
            string filePath,
            IProgress<double> progress,
            CancellationToken token = default)
        {
            try
            {
                var client = await GetClientAsync();

                _logger.LogInformation(
                    "Preparing download for video '{Url}'. Output file: {FilePath}.",
                    videoStreamInfo.Url,
                    filePath);

                var streams = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };

                var conversionRequest = new ConversionRequestBuilder(filePath)
                    .SetFFmpegPath(_ffmpegService.Path)
                    .Build();

                await client.Videos.DownloadAsync(streams, conversionRequest, progress, token);

                _logger.LogInformation(
                    "Video download completed successfully. File saved at {FilePath}.",
                    filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while downloading video from '{Url}'. Output file: {FilePath}.",
                    videoStreamInfo.Url,
                    filePath);

                throw;
            }
        }

        private async Task<StreamManifest> FetchManifestInternal(string videoId, CancellationToken token = default)
        {
            var client = await GetClientAsync();
            var manifest = await client.Videos.Streams.GetManifestAsync(videoId, token);

            _logger.LogInformation("Manifest successfully downloaded for video [{videoId}].", videoId);

            return manifest;
        }
    }
}