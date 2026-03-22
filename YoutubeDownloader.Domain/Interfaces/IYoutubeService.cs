using YoutubeDownloader.Domain.Commands;
using YoutubeDownloader.Domain.ViewModel;

namespace YoutubeDownloader.Domain.Interfaces
{
    public interface IYoutubeService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(DownloadManifestCommand command, CancellationToken token);
        Task<DownloadFileViewModel> DownloadFileAsync(DownloadCommand command, IProgress<double> progress, CancellationToken token);
    }
}
