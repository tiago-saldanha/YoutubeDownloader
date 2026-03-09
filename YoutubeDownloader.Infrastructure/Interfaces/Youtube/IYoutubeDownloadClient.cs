using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Infrastructure.Interfaces.Youtube
{
    public interface IYoutubeDownloadClient
    {
        Task<Video> GetVideoAsync(string url, CancellationToken token = default);
        Task<StreamManifest> GetManifestAsync(string videoId, CancellationToken token = default);
        Task DownloaAudioAsync(IStreamInfo streamInfo, string filePath, IProgress<double> progress, CancellationToken token = default);
        Task DownloadVideoAsync(IStreamInfo audioStreamInfo, IStreamInfo videoStreamInfo, string filePath, IProgress<double> progress, CancellationToken token = default);
    }
}
