using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Domain.ViewModel;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Domain.Interfaces
{
    public interface IYoutubeService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(string url);
        Task<DownloadStreamViewModel> DownloadVideoAsync(StreamManifest manifest, DownloadCommand command);
        Task<DownloadStreamViewModel> DownloadAudioAsync(StreamManifest manifest, DownloadCommand command);
        Task ConverterAsync(string file);
    }
}
