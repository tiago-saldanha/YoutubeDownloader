using Microsoft.Extensions.Caching.Memory;
using YoutubeDownload.Application.Commands;

namespace YoutubeDownload.Blazor.Cache
{
    public class DownloadRequestCache(IMemoryCache cache)
    {
        public string Store(DownloadCommand command)
        {
            var id = Guid.NewGuid().ToString();
            cache.Set(id, command, TimeSpan.FromMinutes(5));
            return id;
        }

        public DownloadCommand? Get(string id)
            => cache.Get<DownloadCommand>(id);

        public void Remove(string id)
            => cache.Remove(id);        
    }
}
