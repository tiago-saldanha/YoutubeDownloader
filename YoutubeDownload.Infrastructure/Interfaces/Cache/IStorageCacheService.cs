using YoutubeDownload.Domain.Commands;

namespace YoutubeDownload.Infrastructure.Interfaces.Cache
{
    public interface IStorageCacheService
    {
        string Store(DownloadCommand command);
        DownloadCommand? Get(string id);
        void Remove(string id);
    }
}