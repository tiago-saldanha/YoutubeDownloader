using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Infrastructure.Interfaces.Youtube
{
    public interface IYoutubeDownloadClient
    {
        Task<Video> GetVideoAsync(string url, CancellationToken token);
        Task<StreamManifest> GetManifestAsync(string videoId, CancellationToken token);
        Task DownloaAudioAsync(IStreamInfo streamInfo, string filePath, IProgress<double> progress, CancellationToken token);
        Task DownloadVideoAsync(IStreamInfo audioStreamInfo, IStreamInfo videoStreamInfo, string filePath, IProgress<double> progress, CancellationToken token);
    }
}
