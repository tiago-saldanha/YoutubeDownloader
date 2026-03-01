using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Infrastructure.Interfaces
{
    public interface IYoutubeDownloadClient
    {
        Task<Video> GetVideoAsync(string url, CancellationToken token = default);
        Task<StreamManifest> GetManifestAsync(string videoId, CancellationToken token = default);
        Task DownloaAudioAsync(IStreamInfo streamInfo, string filePath, CancellationToken token = default);
    }
}
