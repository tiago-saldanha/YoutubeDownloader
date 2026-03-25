using YoutubeDownloader.Domain.Commands;
using YoutubeDownloader.Domain.ViewModel;

namespace YoutubeDownloader.Core.Interfaces
{
    public interface IYoutubeAppService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(DownloadManifestCommand command, CancellationToken cancellationToken);
        Task<DownloadFileViewModel> DownloadFileAsync(DownloadCommand command, IProgress<double> progress, CancellationToken cancellationToken);
    }
}
