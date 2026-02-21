using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.ViewModel;
using Microsoft.Extensions.Logging;
using YoutubeDownload.Application.Exceptions;

namespace YoutubeDownload.Application.Services
{
    public class YoutubeService : IYoutubeService
    {
        private readonly YoutubeClient _client;
        private readonly string OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "downloads");
        private readonly ILogger<YoutubeService> _logger;

        public YoutubeService(YoutubeClient client, ILogger<YoutubeService> logger)
        {
            _client = client;
            _logger = logger;
            CreateOutputDirectory();
        }

        public async Task<StreamManifestViewModel> DownloadManifestAsync(string url)
        {
            try
            {
                _logger.LogInformation("Starting manifest download for video [{Url}].", url);

                var video = await _client.Videos.GetAsync(url);
                var manifest = await _client.Videos.Streams.GetManifestAsync(video.Id);

                var result = StreamManifestViewModel.Create(manifest, video);

                _logger.LogInformation("Manifest successfully downloaded for video [{Url}].", url);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while downloading manifest for video [{Url}].", url);

                throw new VideoManifestDownloadException(url, ex);
            }
        }

        public async Task<DownloadStreamViewModel> DownloadAsync(DownloadCommand command)
        {
            try
            {
                _logger.LogInformation(
                    "Starting video download [{Title}] (ID: {VideoId}).",
                    command.Title,
                    command.VideoId);

                var manifest = await _client.Videos.Streams
                    .GetManifestAsync(command.VideoId);

                var filePath =  command.IsAudioOnly
                    ? await DownloadAudio(manifest, command, command.Title)
                    : await DownloadVideo(manifest, command, command.Title);

                return DownloadStreamViewModel.Create(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while downloading video [{Title}] (ID: {VideoId}).",
                    command.Title,
                    command.VideoId);

                throw new VideoDownloadException(
                    command.VideoId,
                    command.Title,
                    ex);
            }
        }

        public async Task ConverterAsync(string filePath) => await Task.Run(() => FfmpegService.ConvertToMp3(filePath));

        private IVideoStreamInfo DownloadVideoStream(StreamManifest manifest, Func<VideoOnlyStreamInfo, bool> predicate) 
            => manifest.GetVideoOnlyStreams().Where(predicate).OrderByDescending(s => s.Size).First();

        private IStreamInfo DownloadAudioStream(StreamManifest manifest, Func<AudioOnlyStreamInfo, bool> predicate) 
            => manifest.GetAudioOnlyStreams().Where(predicate).OrderByDescending(s => s.Size).First() ?? GetBestAudioStreamInfo(manifest);

        private IStreamInfo GetBestAudioStreamInfo(StreamManifest manifest) => manifest.GetAudioStreams().GetWithHighestBitrate();

        private async Task<string> DownloadVideo(StreamManifest manifest, DownloadCommand command, string title)
        {
            var audioStream = DownloadAudioStream(manifest, s => s.Container.Name == command.ContainerName);
            _logger.LogInformation($"Download do Stream de Audio realizado com sucesso [{audioStream.Container.Name}].");

            var file = Path.Combine(OutputDirectory, $"{title}.{audioStream.Container.Name}");
            var videoStream = DownloadVideoStream(manifest, s => s.Container.ToString() == command.ContainerName && s.VideoQuality.Label.Contains(command.Resolution));
            _logger.LogInformation($"Download do Stream de Video realizado com sucesso [{videoStream.Container.Name}].");

            var streams = new IStreamInfo[2] { audioStream, videoStream };
            _logger.LogInformation($"Iniciando Download do Video [{command.Title}].");

            if (File.Exists(file)) File.Delete(file);

            await _client.Videos.DownloadAsync(streams, new ConversionRequestBuilder(file).SetFFmpegPath(FfmpegService.Path).Build());
            _logger.LogInformation($"Download do Video realizado com sucesso [{file}].");

            return file;
        }

        private async Task<string> DownloadAudio(StreamManifest manifest, DownloadCommand command, string title)
        {
            var audioStream = DownloadAudioStream(manifest, s => s.AudioCodec == command.AudioCodec && s.Container.Name == command.ContainerName);
            _logger.LogInformation($"Download do Stream de Audio realizado com sucesso [{audioStream.Container.Name}].");

            var file = Path.Combine(OutputDirectory, $"{title}.{audioStream.Container.Name}");
            await _client.Videos.Streams.DownloadAsync(audioStream, file);

            _logger.LogInformation($"Download do Audio realizado com sucesso [{file}].");
            return file;
        }

        private void CreateOutputDirectory()
        {
            if (!Directory.Exists(OutputDirectory)) Directory.CreateDirectory(OutputDirectory);
        }
    }
}
