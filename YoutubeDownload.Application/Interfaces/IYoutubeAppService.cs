using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Domain.ViewModel;

namespace YoutubeDownload.Application.Interfaces
{
    public interface IYoutubeAppService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(string url);
        Task<DownloadStreamViewModel> DownloadAsync(DownloadCommand command);
    }
}
