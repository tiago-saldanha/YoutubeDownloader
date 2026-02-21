using Microsoft.AspNetCore.Mvc;
using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.Interfaces;
using YoutubeDownload.Web.Models;

namespace YoutubeDownload.Web.Controllers
{
    public class YoutubeController(IYoutubeService service)
        : Controller
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
                var manifest = await service.DownloadManifestAsync(model.Url);

                if (manifest.Streams.Count > 0)
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
                model.Message = $"Erro: {ex.InnerException?.Message ?? ex.Message}";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Download(StreamViewModel stream)
        {
            try
            {
                var command = new DownloadCommand(
                    stream.VideoId,
                    stream.Title,
                    stream.ContainerName,
                    stream.VideoCodec,
                    stream.Resolution,
                    stream.AudioCodec,
                    stream.IsAudioOnly
                );

                var downloadStream = await service.DownloadAsync(command);
                return File(downloadStream.FileBytes, downloadStream.ContentType, downloadStream.FileName);
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
