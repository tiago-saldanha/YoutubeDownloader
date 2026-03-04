using YoutubeDownload.Domain.ViewModel;

namespace YoutubeDownload.Infrastructure.Interfaces.Cache
{
    public interface IStorageCacheService
    {
        void Store(DownloadFileViewModel download);
        DownloadFileViewModel? Get(string id);
        void Remove(string id);
    }
}