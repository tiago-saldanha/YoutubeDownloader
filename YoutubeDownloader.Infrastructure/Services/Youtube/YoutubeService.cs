using Microsoft.Extensions.Logging;
using YoutubeDownloader.Domain.Commands;
using YoutubeDownloader.Domain.Interfaces;
using YoutubeDownloader.Domain.ViewModel;
using YoutubeDownloader.Infrastructure.Helpers;
using YoutubeDownloader.Infrastructure.Interfaces.Cache;
using YoutubeDownloader.Infrastructure.Interfaces.Youtube;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Infrastructure.Services.Youtube
{
    public class YoutubeService(
        IYoutubeDownloadClient client, 
        IStorageCacheService cache, 
        ILogger<YoutubeService> logger) : IYoutubeService
    {
        public async Task<StreamManifestViewModel> DownloadManifestAsync(
            DownloadManifestCommand command, 
            CancellationToken token)
        {
            var video = await client.GetVideoAsync(command.Url, token);
            var thumbnail = video.Thumbnails.GetWithHighestResolution().Url;
            var manifest = await client.GetManifestAsync(video.Id, token);
            return StreamManifestViewModel.Create(manifest, video, thumbnail);
        }

        public async Task<DownloadFileViewModel> DownloadFileAsync(
            DownloadCommand command, 
            IProgress<double> progress, 
            CancellationToken token)
        {
            return command.IsAudioOnly
                   ? await DownloadAudioFileAsync(command, progress, token)
                   : await DownloadVideoFileAsync(command, progress, token);
        }

        private async Task<DownloadFileViewModel> DownloadAudioFileAsync(
            DownloadCommand command, 
            IProgress<double> progress, 
            CancellationToken token)
        {
            var manifest = await client.GetManifestAsync(command.VideoId, token);
            var audioStream = GetAudioStream(manifest, s => s.AudioCodec == command.AudioCodec && s.Container.Name == command.ContainerName, command.Title);
            var filePath = FileSystemManager.CreateFile(audioStream.Container.Name);

            await client.DownloaAudioAsync(audioStream, filePath, progress, token);
            var download = DownloadFileViewModel.Create(filePath, command.Title, audioStream.Container.Name);
            cache.Store(download);

            return download;
        }

        private async Task<DownloadFileViewModel> DownloadVideoFileAsync(
            DownloadCommand command, 
            IProgress<double> progress, 
            CancellationToken token)
        {
            var manifest = await client.GetManifestAsync(command.VideoId, token);
            var audioStream = GetAudioStream(manifest, s => s.Container.Name == command.ContainerName, command.Title);
            var videoStream = GetVideoStream(manifest, command);

            var filePath = FileSystemManager.CreateFile(audioStream.Container.Name);

            await client.DownloadVideoAsync(audioStream, videoStream, filePath, progress, token);
            var download = DownloadFileViewModel.Create(filePath, command.Title, audioStream.Container.Name);
            cache.Store(download);

            return download;
        }

        private VideoOnlyStreamInfo GetVideoStream(
            StreamManifest manifest, 
            DownloadCommand command)
        {
            logger.LogInformation(
                "Selecting video stream. Resolution: {Resolution}, Container: {Container}.", 
                command.Resolution, 
                command.ContainerName);

            var videoStream = manifest
                .GetVideoOnlyStreams()
                .Where(s => s.Container.ToString() == command.ContainerName && s.VideoQuality.Label.Contains(command.Resolution))
                .OrderByDescending(s => s.Size)
                .First();

            logger.LogInformation(
                "Video stream selected. Container: {Container}, Quality: {Quality}.", 
                videoStream.Container.Name, 
                videoStream.VideoQuality.Label);

            return videoStream;
        }
            
        private IStreamInfo GetAudioStream(
            StreamManifest manifest, 
            Func<AudioOnlyStreamInfo, bool> predicate, 
            string title)
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
