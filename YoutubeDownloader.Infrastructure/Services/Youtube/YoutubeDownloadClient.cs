using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Web;
using YoutubeDownloader.Infrastructure.Helpers;
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
        private readonly IMemoryCache _cache;
        private readonly ILogger<YoutubeDownloadClient> _logger;

        public YoutubeDownloadClient(
            IFfmpegService ffmpegService,
            IMemoryCache cache,
            ILogger<YoutubeDownloadClient> logger)
        {
            _ffmpegService = ffmpegService;
            _cache = cache;
            _logger = logger;
            _clientLazy = new Lazy<Task<YoutubeClient>>(CreateClientAsync);
        }

        public async Task<Video> GetVideoAsync(
            string url,
            CancellationToken token)
        {
            var videoId = GetVideoId(url);
            var key = $"video:{videoId}";

            try
            {
                return await _cache.GetOrCreateAsync(key, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);

                    _logger.LogInformation("Cache MISS (Video) [{url}]", url);

                    return await FetchVideoInternalAsync(url, token);
                });
            }
            catch
            {
                _cache.Remove(url);
                throw;
            }
        }

        public async Task<StreamManifest> GetManifestAsync(
            string videoId, 
            CancellationToken token)
        {
            var key = $"manifest:{videoId}";
            try
            {
                return await _cache.GetOrCreateAsync(key, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                    _logger.LogInformation("Cache MISS for video [{videoId}]. Fetching manifest...", videoId);

                    return await FetchManifestInternalAsync(videoId, token);
                });
            }
            catch
            {
                _cache.Remove(videoId);
                throw;
            }
        }

        public async Task DownloadAudioAsync(
            IStreamInfo streamInfo,
            string filePath,
            IProgress<double> progress,
            CancellationToken token)
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
            CancellationToken token)
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
                    .SetPreset(ConversionPreset.UltraFast)
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

        private async Task<Video> FetchVideoInternalAsync(
            string url,
            CancellationToken token)
        {
            var client = await GetClientAsync();
            _logger.LogInformation("Starting video fetch for [{Url}].", url);
            return await client.Videos.GetAsync(url, token);
        }

        private async Task<StreamManifest> FetchManifestInternalAsync(
            string videoId,
            CancellationToken token)
        {
            var client = await GetClientAsync();
            var manifest = await client.Videos.Streams.GetManifestAsync(videoId, token);

            _logger.LogInformation("Manifest successfully downloaded for video [{videoId}].", videoId);

            return manifest;
        }

        private static string GetVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));

            var uri = new Uri(url);

            if (uri.Host.Contains("youtu.be"))
            {
                return uri.AbsolutePath.Trim('/');
            }

            var query = HttpUtility.ParseQueryString(uri.Query);
            var videoId = query["v"];

            if (!string.IsNullOrEmpty(videoId))
                return videoId;

            throw new InvalidOperationException("Invalid YouTube URL format.");
        }

        private async Task<YoutubeClient> CreateClientAsync()
        {
            var container = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = container, UseCookies = true };

            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", YoutubeDownloaderInfrastructure.UserAgent);

            var requestUri = new Uri(YoutubeDownloaderInfrastructure.YoutubeUrl);
            await httpClient.GetAsync(requestUri);

            var cookies = container.GetCookies(requestUri).ToList().AsReadOnly();

            return new YoutubeClient(httpClient, cookies);
        }

        private async Task<YoutubeClient> GetClientAsync()
            => await _clientLazy.Value;
    }
}