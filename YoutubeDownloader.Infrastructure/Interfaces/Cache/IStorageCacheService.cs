using YoutubeDownloader.Domain.ViewModel;

namespace YoutubeDownloader.Infrastructure.Interfaces.Cache
{
    public interface IStorageCacheService
    {
        void Store(DownloadFileViewModel download);
        DownloadFileViewModel? Get(string id);
        void Remove(string id);
    }
}