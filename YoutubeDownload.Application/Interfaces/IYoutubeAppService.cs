using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Domain.ViewModel;

namespace YoutubeDownload.Application.Interfaces
{
    public interface IYoutubeAppService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(DownloadManifestCommand command);
        Task<DownloadStreamViewModel> DownloadStreamAsync(DownloadCommand command);
    }
}
