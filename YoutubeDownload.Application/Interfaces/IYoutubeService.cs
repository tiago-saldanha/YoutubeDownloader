using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.ViewModel;

namespace YoutubeDownload.Application.Interfaces
{
    public interface IYoutubeService
    {
        Task<IEnumerable<StreamManifestViewModel>> DownloadManifest(string url);
        Task<string> Download(DownloadCommand command);
        Task Converter(string file);
    }
}
