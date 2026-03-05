using Microsoft.AspNetCore.Mvc;
using YoutubeDownload.Domain.Commands;
using YoutubeDownload.Core.Interfaces;
using YoutubeDownload.Web.Models;

namespace YoutubeDownload.Web.Controllers
{
    public class YoutubeController(IYoutubeAppService service) : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new YoutubePageViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(YoutubePageViewModel model)
        {
            if (string.IsNullOrEmpty(model.Url))
            {
                model.Message = "Por favor, insira uma URL válida.";
            }

            model.Message = "Pesquisando...";
            model.Loading = true;

            try
            {
                var command = new DownloadManifestCommand(model.Url);
                var manifest = await service.DownloadManifestAsync(command);

                if (manifest.Streams.Any())
                {
                    model.Streams = manifest.Streams.Select(s => StreamViewModel.Create(s, manifest)).ToList();

                    model.Loading = false;
                    model.Message = "";
                }
                else
                {
                    model.Message = "Nenhum stream encontrado";
                }
            }
            catch (Exception ex)
            {
                TempData["Alert.Type"] = "danger";
                TempData["Alert.Message"] = ex.InnerException?.Message ?? ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Download(StreamViewModel model)
        {
            try
            {
                var command = new DownloadCommand(
                    model.VideoId,
                    model.Title,
                    model.ContainerName,
                    model.VideoCodec,
                    model.Resolution,
                    model.AudioCodec,
                    model.IsAudioOnly
                );

                var download = await service.DownloadFileAsync(command, null, new CancellationToken());
                var stream = new FileStream(download.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return new FileStreamResult(stream, download.ContentType)
                {
                    FileDownloadName = download.FileName,
                };
            }
            catch (Exception ex)
            {
                TempData["Alert.Type"] = "danger";
                TempData["Alert.Message"] = ex.InnerException?.Message ?? ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
