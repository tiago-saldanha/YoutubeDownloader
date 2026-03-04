using Microsoft.Extensions.Caching.Memory;
using YoutubeDownload.Domain.ViewModel;
using YoutubeDownload.Infrastructure.Interfaces.Cache;

namespace YoutubeDownload.Infrastructure.Services.Cache
{
    public class StorageCacheService(IMemoryCache cache) : IStorageCacheService
    {

        public void Store(DownloadFileViewModel command)
            => cache.Set(command.Id, command, TimeSpan.FromMinutes(5));

        public DownloadFileViewModel? Get(string id)
            => cache.Get<DownloadFileViewModel>(id);

        public void Remove(string id)
            => cache.Remove(id);        
    }
}
