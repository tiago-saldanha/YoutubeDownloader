using Microsoft.AspNetCore.Mvc;
using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Infrastructure.Interfaces;

namespace YoutubeDownload.Blazor.Controllers
{
    [ApiController]
    [Route("api/download")]
    public class DownloadController(IStorageCacheService cache) : ControllerBase
    {
        [HttpPost("prepare")]
        public IActionResult Prepare([FromBody] DownloadCommand command)
        {
            var id = cache.Store(command);
            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Download([FromServices] IYoutubeAppService service, string id)
        {
            var output = cache.Get(id);

            if (output is DownloadCommand command)
            {
                var download = await service.DownloadAsync(command);
                cache.Remove(id);
                return File(download.FileBytes, download.ContentType, download.FileName);
            }

            return NotFound();
        }
    }
}
