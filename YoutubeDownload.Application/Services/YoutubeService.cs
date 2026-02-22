using Microsoft.Extensions.Logging;
using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Application.ViewModel;
using YoutubeDownload.Infrastructure.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Application.Services
{
    public class YoutubeService(YoutubeClient client, IFfmpegService ffmpegService, ILogger<YoutubeService> logger) 
        : IYoutubeService
    {
        private string OutputDirectory => GetOutputDirectory();

        public async Task<StreamManifestViewModel> DownloadManifestAsync(string url)
        {
            logger.LogInformation("Starting manifest download for video [{Url}].", url);
            var video = await client.Videos.GetAsync(url);
            
            var manifest = await client.Videos.Streams.GetManifestAsync(video.Id);
            logger.LogInformation("Manifest successfully downloaded for video [{Url}].", url);
            
            return StreamManifestViewModel.Create(manifest, video);
        }

        public async Task<DownloadStreamViewModel> DownloadVideoAsync(StreamManifest manifest, DownloadCommand command)
        {
            logger.LogInformation("Selecting audio stream for video '{Title}'.", command.Title);

            var audioStream = GetAudioStream(manifest, s => s.Container.Name == command.ContainerName);

            logger.LogInformation("Audio stream selected. Container: {Container}.", audioStream.Container.Name);

            logger.LogInformation("Selecting video stream. Resolution: {Resolution}, Container: {Container}.", command.Resolution, command.ContainerName);

            var videoStream = GetVideoStream(manifest, s => s.Container.ToString() == command.ContainerName && s.VideoQuality.Label.Contains(command.Resolution));

            logger.LogInformation("Video stream selected. Container: {Container}, Quality: {Quality}.", videoStream.Container.Name, videoStream.VideoQuality.Label);

            var filePath = CreateFilePath(audioStream.Container.Name);
            RemoveExistingFile(filePath);

            logger.LogInformation("Preparing download for video '{Title}'. Output file: {FilePath}.", command.Title, filePath);

            var streams = new IStreamInfo[] { audioStream, videoStream };

            await client.Videos.DownloadAsync(streams, new ConversionRequestBuilder(filePath).SetFFmpegPath(ffmpegService.Path).Build());

            logger.LogInformation("Video download completed successfully. File saved at {FilePath}.", filePath);

            var fileName = $"{command.Title}.{audioStream.Container.Name}";
            var download = DownloadStreamViewModel.Create(filePath, fileName);

            RemoveExistingFile(filePath);

            return download;
        }

        public async Task<DownloadStreamViewModel> DownloadAudioAsync(StreamManifest manifest, DownloadCommand command)
        {
            var audioStream = GetAudioStream(manifest, s => s.AudioCodec == command.AudioCodec && s.Container.Name == command.ContainerName);
            logger.LogInformation("Audio stream selected. Container: {Container}.", audioStream.Container.Name);

            var filePath = CreateFilePath(audioStream.Container.Name);
            await client.Videos.Streams.DownloadAsync(audioStream, filePath);

            var fileName = $"{command.Title}.{audioStream.Container.Name}";

            logger.LogInformation("Audio download completed successfully. File saved at {FilePath}.", filePath);

            var download = DownloadStreamViewModel.Create(filePath, fileName);

            RemoveExistingFile(filePath);

            return download;
        }

        public async Task ConverterAsync(string filePath)
            => await Task.Run(() => ffmpegService.ConvertToMp3(filePath));

        private IVideoStreamInfo GetVideoStream(StreamManifest manifest, Func<VideoOnlyStreamInfo, bool> predicate)
            => manifest.GetVideoOnlyStreams().Where(predicate).OrderByDescending(s => s.Size).First();

        private IStreamInfo GetAudioStream(StreamManifest manifest, Func<AudioOnlyStreamInfo, bool> predicate)
            => manifest.GetAudioOnlyStreams().Where(predicate).OrderByDescending(s => s.Size).First() ?? GetBestAudioStreamInfo(manifest);

        private IStreamInfo GetBestAudioStreamInfo(StreamManifest manifest)
            => manifest.GetAudioStreams().GetWithHighestBitrate();

        private string CreateFilePath(string containerName)
            => Path.Combine(OutputDirectory, $"{Guid.NewGuid()}.{containerName}");

        private void RemoveExistingFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                logger.LogInformation("File removed after download. FilePath: {FilePath}.", filePath);
            }
        }

        private string GetOutputDirectory()
        {
            var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "downloads");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                logger.LogInformation("Output directory created at {OutputDirectory}.", outputDirectory);
            }
            return outputDirectory;
        }
    }
}
