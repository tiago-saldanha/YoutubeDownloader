using YoutubeDownloader.Domain.Commands;
using YoutubeDownloader.Domain.ViewModel;

namespace YoutubeDownloader.Domain.Interfaces
{
    public interface IYoutubeService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(DownloadManifestCommand command, CancellationToken cancellationToken);
        Task<DownloadFileViewModel> DownloadFileAsync(DownloadCommand command, IProgress<double> progress, CancellationToken cancellationToken);
    }
}
