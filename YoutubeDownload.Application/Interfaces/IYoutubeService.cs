using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.ViewModel;

namespace YoutubeDownload.Application.Interfaces
{
    public interface IYoutubeService
    {
        Task<IEnumerable<StreamManifestViewModel>> DownloadManifestAsync(string url);
        Task<string> DownloadAsync(DownloadCommand command);
        Task ConverterAsync(string file);
    }
}
