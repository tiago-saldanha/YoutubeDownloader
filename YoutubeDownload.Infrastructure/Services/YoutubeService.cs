using Microsoft.Extensions.Logging;
using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Domain.Interfaces;
using YoutubeDownload.Domain.ViewModel;
using YoutubeDownload.Infrastructure.Interfaces;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Application.Services
{
    public class YoutubeService(IYoutubeDownloadClient youtubeClient, ILogger<YoutubeService> logger)
        : IYoutubeService
    {
        private string OutputDirectory => GetOutputDirectory();

        public async Task<StreamManifestViewModel> DownloadManifestAsync(string url)
        {
            var video = await youtubeClient.GetVideoAsync(url);
            var manifest = await youtubeClient.GetManifestAsync(video.Id);
            return StreamManifestViewModel.Create(manifest, video);
        }

        public async Task<DownloadStreamViewModel> DownloadVideoAsync(StreamManifest manifest, DownloadCommand command)
        {
            var audioStream = GetAudioStream(manifest, s => s.Container.Name == command.ContainerName, command.Title);
            var videoStream = GetVideoStream(manifest, s => s.Container.ToString() == command.ContainerName && s.VideoQuality.Label.Contains(command.Resolution), command);

            var filePath = CreateFilePath(audioStream.Container.Name);
            RemoveExistingFile(filePath);
            
            await youtubeClient.DownloadVideoAsync(audioStream, videoStream, filePath);

            var fileName = $"{command.Title}.{audioStream.Container.Name}";
            var download = DownloadStreamViewModel.Create(filePath, fileName);

            RemoveExistingFile(filePath);

            return download;
        }

        public async Task<DownloadStreamViewModel> DownloadAudioAsync(StreamManifest manifest, DownloadCommand command)
        {
            var audioStream = GetAudioStream(manifest, s => s.AudioCodec == command.AudioCodec && s.Container.Name == command.ContainerName, command.Title);
            var filePath = CreateFilePath(audioStream.Container.Name);
            
            await youtubeClient.DownloaAudioAsync(audioStream, filePath);
            
            var fileName = $"{command.Title}.{audioStream.Container.Name}";
            var download = DownloadStreamViewModel.Create(filePath, fileName);
            
            RemoveExistingFile(filePath);
            
            return download;
        }

        private VideoOnlyStreamInfo GetVideoStream(StreamManifest manifest, Func<VideoOnlyStreamInfo, bool> predicate, DownloadCommand command)
        {
            logger.LogInformation("Selecting video stream. Resolution: {Resolution}, Container: {Container}.", command.Resolution, command.ContainerName);
            var videoStream = manifest
                .GetVideoOnlyStreams()
                .Where(predicate)
                .OrderByDescending(s => s.Size)
                .First();

            logger.LogInformation("Video stream selected. Container: {Container}, Quality: {Quality}.", videoStream.Container.Name, videoStream.VideoQuality.Label);
            return videoStream;
        }
            
        private IStreamInfo GetAudioStream(StreamManifest manifest, Func<AudioOnlyStreamInfo, bool> predicate, string title)
        {
            logger.LogInformation("Selecting audio stream for video '{title}'.", title);
            var audioStream = manifest
                .GetAudioOnlyStreams()
                .Where(predicate)
                .OrderByDescending(s => s.Size)
                .First() ??
                    GetBestAudioStreamInfo(manifest);

            logger.LogInformation("Audio stream selected. Container: {Container}.", audioStream.Container.Name);
            return audioStream;
        }

        private static IStreamInfo GetBestAudioStreamInfo(StreamManifest manifest)
            => manifest.GetAudioOnlyStreams().GetWithHighestBitrate();

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
