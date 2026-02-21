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
                _logger.LogInformation("Starting video download [{Title}] (ID: {VideoId}).", command.Title, command.VideoId);

                var manifest = await _client.Videos.Streams.GetManifestAsync(command.VideoId);

                return command.IsAudioOnly
                    ? await DownloadAudioAsync(manifest, command)
                    : await DownloadVideoAsync(manifest, command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while downloading video [{Title}] (ID: {VideoId}).", command.Title, command.VideoId);

                throw new VideoDownloadException(command.VideoId, command.Title, ex);
            }
        }

        public async Task ConverterAsync(string filePath)
            => await Task.Run(() => FfmpegService.ConvertToMp3(filePath));

        private async Task<DownloadStreamViewModel> DownloadVideoAsync(StreamManifest manifest, DownloadCommand command)
        {
            _logger.LogInformation("Selecting audio stream for video '{Title}'.", command.Title);

            var audioStream = GetAudioStream(manifest, s => s.Container.Name == command.ContainerName);

            _logger.LogInformation("Audio stream selected. Container: {Container}.", audioStream.Container.Name);

            _logger.LogInformation("Selecting video stream. Resolution: {Resolution}, Container: {Container}.", command.Resolution, command.ContainerName);

            var videoStream = GetVideoStream(manifest, s => s.Container.ToString() == command.ContainerName && s.VideoQuality.Label.Contains(command.Resolution));

            _logger.LogInformation("Video stream selected. Container: {Container}, Quality: {Quality}.", videoStream.Container.Name, videoStream.VideoQuality.Label);

            var filePath = CreateFilePath(audioStream.Container.Name);

            _logger.LogInformation("Preparing download for video '{Title}'. Output file: {FilePath}.", command.Title, filePath);

            RemoveExistingFile(filePath);

            var streams = new IStreamInfo[] { audioStream, videoStream };

            await _client.Videos.DownloadAsync(streams, new ConversionRequestBuilder(filePath).SetFFmpegPath(FfmpegService.Path).Build());

            _logger.LogInformation("Video download completed successfully. File saved at {FilePath}.", filePath);

            var fileName = $"{command.Title}.{audioStream.Container.Name}";
            var download = DownloadStreamViewModel.Create(filePath, fileName);

            RemoveExistingFile(filePath);

            return download;
        }

        private async Task<DownloadStreamViewModel> DownloadAudioAsync(StreamManifest manifest, DownloadCommand command)
        {
            var audioStream = GetAudioStream(manifest, s => s.AudioCodec == command.AudioCodec && s.Container.Name == command.ContainerName);
            _logger.LogInformation($"Download do Stream de Audio realizado com sucesso [{audioStream.Container.Name}].");

            var filePath = CreateFilePath(audioStream.Container.Name);
            await _client.Videos.Streams.DownloadAsync(audioStream, filePath);

            var fileName = $"{command.Title}.{audioStream.Container.Name}";

            _logger.LogInformation($"Download do Audio realizado com sucesso [{filePath}].");

            var download = DownloadStreamViewModel.Create(filePath, fileName);

            RemoveExistingFile(filePath);

            return download;
        }

        private IVideoStreamInfo GetVideoStream(StreamManifest manifest, Func<VideoOnlyStreamInfo, bool> predicate)
            => manifest.GetVideoOnlyStreams().Where(predicate).OrderByDescending(s => s.Size).First();

        private IStreamInfo GetAudioStream(StreamManifest manifest, Func<AudioOnlyStreamInfo, bool> predicate)
            => manifest.GetAudioOnlyStreams().Where(predicate).OrderByDescending(s => s.Size).First() ?? GetBestAudioStreamInfo(manifest);

        private IStreamInfo GetBestAudioStreamInfo(StreamManifest manifest)
            => manifest.GetAudioStreams().GetWithHighestBitrate();

        private void CreateOutputDirectory()
        {
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
                _logger.LogInformation("Output directory created at {OutputDirectory}.", OutputDirectory);
            }
        }

        private string CreateFilePath(string containerName)
            => Path.Combine(OutputDirectory, $"{Guid.NewGuid()}.{containerName}");

        private void RemoveExistingFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File removed after download. FilePath: {FilePath}.", filePath);
            }
        }
    }
}
