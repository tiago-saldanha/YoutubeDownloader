using Microsoft.Extensions.Logging;
using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Domain.Interfaces;
using YoutubeDownload.Domain.ViewModel;
using YoutubeDownload.Infrastructure.Helpers;
using YoutubeDownload.Infrastructure.Interfaces;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Infrastructure.Services
{
    public class YoutubeService(IYoutubeDownloadClient client, ILogger<YoutubeService> logger)
        : IYoutubeService
    {
        public async Task<StreamManifestViewModel> DownloadManifestAsync(string url)
        {
            var video = await client.GetVideoAsync(url);
            var manifest = await client.GetManifestAsync(video.Id);
            return StreamManifestViewModel.Create(manifest, video);
        }

        public async Task<DownloadStreamViewModel> DownloadStreamAsync(DownloadCommand command)
        {
            var manifest = await client.GetManifestAsync(command.VideoId);

            return command.IsAudioOnly
                   ? await DownloadAudioStreamAsync(manifest, command)
                   : await DownloadVideoStreamAsync(manifest, command);
        }

        private async Task<DownloadStreamViewModel> DownloadVideoStreamAsync(StreamManifest manifest, DownloadCommand command)
        {
            var audioStream = GetAudioStream(manifest, s => s.Container.Name == command.ContainerName, command.Title);
            var videoStream = GetVideoStream(manifest, command);

            var filePath = FileSystemManager.CreateFile(audioStream.Container.Name);
            
            await client.DownloadVideoAsync(audioStream, videoStream, filePath);
            var download = DownloadStreamViewModel.Create(filePath, command.Title, audioStream.Container.Name);
            
            FileSystemManager.RemoveFile(filePath);

            return download;
        }

        private async Task<DownloadStreamViewModel> DownloadAudioStreamAsync(StreamManifest manifest, DownloadCommand command)
        {
            var audioStream = GetAudioStream(manifest, s => s.AudioCodec == command.AudioCodec && s.Container.Name == command.ContainerName, command.Title);
            var filePath = FileSystemManager.CreateFile(audioStream.Container.Name);
            
            await client.DownloaAudioAsync(audioStream, filePath);
            var download = DownloadStreamViewModel.Create(filePath, command.Title, audioStream.Container.Name);
            
            FileSystemManager.RemoveFile(filePath);
            
            return download;
        }

        private VideoOnlyStreamInfo GetVideoStream(StreamManifest manifest, DownloadCommand command)
        {
            logger.LogInformation("Selecting video stream. Resolution: {Resolution}, Container: {Container}.", command.Resolution, command.ContainerName);
            var videoStream = manifest
                .GetVideoOnlyStreams()
                .Where(s => s.Container.ToString() == command.ContainerName && s.VideoQuality.Label.Contains(command.Resolution))
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
    }
}
