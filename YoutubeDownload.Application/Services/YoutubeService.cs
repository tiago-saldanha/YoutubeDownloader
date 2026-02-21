using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;
using YoutubeDownload.Application.Extensions;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.ViewModel;
using Microsoft.Extensions.Logging;

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

        public async Task<IEnumerable<StreamManifestViewModel>> DownloadManifestAsync(string url)
        {
            try
            {
                _logger.LogInformation($"Iniciando o Download do manifesto do vídeo [{url}].");
                var video = await _client.Videos.GetAsync(url);

                var manifest = await _client.Videos.Streams.GetManifestAsync(video.Id);
                var streams = manifest.Streams.Select(stream => StreamManifestViewModel.Create(stream, url)).ToList();

                _logger.LogInformation($"Download dos Streams realizados com sucesso [{url}]");
                return streams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar baixar o manifesto do video [{url}].");
                throw;
            }
        }

        public async Task<string> DownloadAsync(DownloadCommand command)
        {
            try
            {
                var video = await _client.Videos.GetAsync(command.Url);
                var title = video.Title.FormaterName();
                _logger.LogInformation($"Iniciando o Download do vídeo [{title}].");

                var manifest = await _client.Videos.Streams.GetManifestAsync(video.Id);
                return command.IsAudioOnly ? await DownloadAudio(manifest, command, title) : await DownloadVideo(manifest, command, title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao tentar baixar o video [{command.Url}].");
                return ex.ToString();
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
            _logger.LogInformation($"Iniciando Download do Video [{command.Url}].");

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
