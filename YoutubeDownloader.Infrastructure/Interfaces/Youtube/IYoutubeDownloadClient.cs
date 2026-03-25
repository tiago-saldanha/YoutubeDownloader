using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Infrastructure.Interfaces.Youtube
{
    public interface IYoutubeDownloadClient
    {
        Task<Video> GetVideoAsync(string url, CancellationToken cancellationToken);
        Task<StreamManifest> GetManifestAsync(string videoId, CancellationToken cancellationToken);
        Task DownloadAudioAsync(IStreamInfo streamInfo, string filePath, IProgress<double> progress, CancellationToken cancellationToken);
        Task DownloadVideoAsync(IStreamInfo audioStreamInfo, IStreamInfo videoStreamInfo, string filePath, IProgress<double> progress, CancellationToken cancellationToken);
    }
}
