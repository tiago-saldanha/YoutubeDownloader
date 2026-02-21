using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.ViewModel;

namespace YoutubeDownload.Application.Interfaces
{
    public interface IYoutubeService
    {
        Task<StreamManifestViewModel> DownloadManifestAsync(string url);
        Task<DownloadStreamViewModel> DownloadAsync(DownloadCommand command);
        Task ConverterAsync(string file);
    }
}
