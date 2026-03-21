using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.Infrastructure.Interfaces.Cache;
using YoutubeDownloader.Domain.ViewModel;

namespace YoutubeDownloader.Blazor.Controllers
{
    [ApiController]
    [Route("api/download")]
    public class DownloadController(IStorageCacheService cache) : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult Download(string id)
        {
            var output = cache.Get(id);

            if (output is DownloadFileViewModel command)
            {
                HttpContext.Items["cleanup-file"] = command.FilePath;

                cache.Remove(id);
                var stream = new FileStream(
                    command.FilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                return new FileStreamResult(stream, command.ContentType)
                {
                    FileDownloadName = command.FileName,
                };
            }

            return NotFound();
        }
    }
}
