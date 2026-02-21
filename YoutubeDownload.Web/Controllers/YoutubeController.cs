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

                if (manifest.Streams.Any())
                {
                    model.Streams = manifest.Streams
                        .Select(s => new StreamViewModel
                        {
                            ContainerName = s.ContainerName,
                            VideoCodec = s.VideoCodec,
                            Resolution = s.Resolution,
                            Size = s.Size,
                            IsAudioOnly = s.IsAudioOnly,
                            AudioCodec = s.AudioCodec,
                            VideoId = manifest.VideoId,
                            Title = manifest.Title,
                        }).ToList();

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
            var command = new DownloadCommand(
                stream.VideoId,
                stream.Title,
                stream.ContainerName,
                stream.VideoCodec,
                stream.Resolution,
                stream.AudioCodec,
                stream.IsAudioOnly
            );

            var filePath = await service.DownloadAsync(command);

            if (!System.IO.File.Exists(filePath))
            {
                TempData["Message"] = "Erro ao baixar o arquivo.";
                return RedirectToAction(nameof(Index));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
