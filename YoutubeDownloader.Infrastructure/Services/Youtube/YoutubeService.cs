using Microsoft.Extensions.Logging;
using YoutubeDownloader.Domain.Commands;
using YoutubeDownloader.Domain.Interfaces;
using YoutubeDownloader.Domain.ViewModel;
using YoutubeDownloader.Infrastructure.Helpers;
using YoutubeDownloader.Infrastructure.Interfaces.Cache;
using YoutubeDownloader.Infrastructure.Interfaces.Youtube;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Infrastructure.Services.Youtube
{
    public class YoutubeService(IYoutubeDownloadClient client, IStorageCacheService cache, ILogger<YoutubeService> logger)
        : IYoutubeService
    {
        public async Task<StreamManifestViewModel> DownloadManifestAsync(DownloadManifestCommand command)
        {
            var video = await client.GetVideoAsync(command.Url);
            var manifest = await client.GetManifestAsync(video.Id);
            return StreamManifestViewModel.Create(manifest, video);
        }

        public async Task<DownloadStreamViewModel> DownloadStreamAsync(DownloadCommand command, IProgress<double> progress, CancellationToken token = default)
        {
            var manifest = await client.GetManifestAsync(command.VideoId, token);

            return command.IsAudioOnly
                   ? await DownloadAudioStreamAsync(manifest, command, progress, token)
                   : await DownloadVideoStreamAsync(manifest, command, progress, token);
        }

        public async Task<DownloadFileViewModel> DownloadFileAsync(DownloadCommand command, IProgress<double> progress, CancellationToken token = default)
        {
            var manifest = await client.GetManifestAsync(command.VideoId, token);

            return command.IsAudioOnly
                   ? await DownloadAudioFileAsync(manifest, command, progress, token)
                   : await DownloadVideoFileAsync(manifest, command, progress, token);
        }

        private async Task<DownloadStreamViewModel> DownloadVideoStreamAsync(StreamManifest manifest, DownloadCommand command, IProgress<double> progress, CancellationToken token = default)
        {
            var audioStream = GetAudioStream(manifest, s => s.Container.Name == command.ContainerName, command.Title);
            var videoStream = GetVideoStream(manifest, command);

            var filePath = FileSystemManager.CreateFile(audioStream.Container.Name);
            
            await client.DownloadVideoAsync(audioStream, videoStream, filePath, progress, token);
            var download = DownloadStreamViewModel.Create(filePath, command.Title, audioStream.Container.Name);
            
            FileSystemManager.RemoveFile(filePath);

            return download;
        }

        private async Task<DownloadStreamViewModel> DownloadAudioStreamAsync(StreamManifest manifest, DownloadCommand command, IProgress<double> progress, CancellationToken token = default)
        {
            var audioStream = GetAudioStream(manifest, s => s.AudioCodec == command.AudioCodec && s.Container.Name == command.ContainerName, command.Title);
            var filePath = FileSystemManager.CreateFile(audioStream.Container.Name);
            
            await client.DownloaAudioAsync(audioStream, filePath, progress, token);
            var download = DownloadStreamViewModel.Create(filePath, command.Title, audioStream.Container.Name);
            
            FileSystemManager.RemoveFile(filePath);
            
            return download;
        }

        private async Task<DownloadFileViewModel> DownloadAudioFileAsync(StreamManifest manifest, DownloadCommand command, IProgress<double> progress, CancellationToken token = default)
        {
            var audioStream = GetAudioStream(manifest, s => s.AudioCodec == command.AudioCodec && s.Container.Name == command.ContainerName, command.Title);
            var filePath = FileSystemManager.CreateFile(audioStream.Container.Name);

            await client.DownloaAudioAsync(audioStream, filePath, progress, token);
            var download = DownloadFileViewModel.Create(filePath, command.Title, audioStream.Container.Name);
            cache.Store(download);

            return download;
        }

        private async Task<DownloadFileViewModel> DownloadVideoFileAsync(StreamManifest manifest, DownloadCommand command, IProgress<double> progress, CancellationToken token = default)
        {
            var audioStream = GetAudioStream(manifest, s => s.Container.Name == command.ContainerName, command.Title);
            var videoStream = GetVideoStream(manifest, command);

            var filePath = FileSystemManager.CreateFile(audioStream.Container.Name);

            await client.DownloadVideoAsync(audioStream, videoStream, filePath, progress, token);
            var download = DownloadFileViewModel.Create(filePath, command.Title, audioStream.Container.Name);
            cache.Store(download);

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
