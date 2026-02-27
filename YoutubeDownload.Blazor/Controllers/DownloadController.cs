using Microsoft.AspNetCore.Mvc;
using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Blazor.Interfaces;

namespace YoutubeDownload.Blazor.Controllers
{
    [ApiController]
    [Route("api/download")]
    public class DownloadController(IYoutubeAppService service, IStorageCache cache) : ControllerBase
    {
        [HttpPost("prepare")]
        public IActionResult Prepare([FromBody] DownloadCommand command)
        {
            var id = cache.Store(command);
            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Download(string id)
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
