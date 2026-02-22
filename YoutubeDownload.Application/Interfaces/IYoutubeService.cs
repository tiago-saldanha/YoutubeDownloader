using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.ViewModel;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Application.Interfaces
{
    public interface IYoutubeService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(string url);
        Task<DownloadStreamViewModel> DownloadVideoAsync(StreamManifest manifest, DownloadCommand command);
        Task<DownloadStreamViewModel> DownloadAudioAsync(StreamManifest manifest, DownloadCommand command);
        Task ConverterAsync(string file);
    }
}
