using YoutubeDownload.Application.Commands;

namespace YoutubeDownload.Blazor.Interfaces
{
    public interface IStorageCache
    {
        string Store(DownloadCommand command);
        DownloadCommand? Get(string id);
        void Remove(string id);
    }
}