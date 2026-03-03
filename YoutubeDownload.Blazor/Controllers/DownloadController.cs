using Microsoft.AspNetCore.Mvc;
using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Core.Interfaces;
using YoutubeDownload.Infrastructure.Interfaces.Cache;

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
                var stream = await service.DownloadStreamAsync(command, null, new CancellationToken());
                cache.Remove(id);
                return File(stream.FileBytes, stream.ContentType, stream.FileName);
            }

            return NotFound();
        }
    }
}
