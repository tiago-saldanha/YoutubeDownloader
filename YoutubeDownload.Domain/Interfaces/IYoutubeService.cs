using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Domain.ViewModel;

namespace YoutubeDownload.Domain.Interfaces
{
    public interface IYoutubeService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(DownloadManifestCommand command);
        Task<DownloadStreamViewModel> DownloadStreamAsync(DownloadCommand command, IProgress<double> progress, CancellationToken token = default);
        Task<DownloadFileViewModel> DownloadFileAsync(DownloadCommand command, IProgress<double> progress, CancellationToken token = default);
    }
}
