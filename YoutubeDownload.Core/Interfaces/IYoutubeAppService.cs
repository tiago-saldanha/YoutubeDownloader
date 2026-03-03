using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Domain.ViewModel;

namespace YoutubeDownload.Core.Interfaces
{
    public interface IYoutubeAppService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(DownloadManifestCommand command);
        Task<DownloadStreamViewModel> DownloadStreamAsync(DownloadCommand command, IProgress<double> progress, CancellationToken token = default);
        Task<DownloadFileViewModel> DownloadFileAsync(DownloadCommand command, IProgress<double> progress, CancellationToken token = default);
    }
}
