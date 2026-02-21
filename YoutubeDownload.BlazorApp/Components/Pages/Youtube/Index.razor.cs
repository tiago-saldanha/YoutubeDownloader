using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using YoutubeDownload.Application.Commands;
using YoutubeDownload.BlazorApp.Models;

namespace YoutubeDownload.BlazorApp.Components.Pages.Youtube
{
    public partial class Index
    {
        private string Url { get; set; }
        
        private List<StreamViewModel> Streams = new();
        
        private bool Loading { get; set; } = true;
        
        private string Message { get; set; } = string.Empty;
        
        [Inject] IJSRuntime JSRuntime { get; set; }

        private async Task GetManifestInfo()
        {
            if (string.IsNullOrEmpty(Url))
            {
                Message = "Por favor, insira uma URL válida.";
                return;
            }

            Message = "Pesquisando...";
            Loading = true;

            try
            {
                var streams = await _service.DownloadManifestAsync(Url);
                if (streams.Any())
                {
                    Streams.Clear();
                    Streams.AddRange(streams.Select(StreamViewModel.Create));
                    Loading = false;
                }
                else
                {
                    Message = "Nenhum stream encontrado.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Erro: {ex.InnerException.Message ?? ex.Message}";
            }
        }

        private async Task SubmitSelection(StreamViewModel stream)
        {
            Message = "Baixando...";
            Loading = true;
            var command = new DownloadCommand(stream.Url, stream.ContainerName, stream.VideoCodec, stream.Resolution, stream.AudioCodec, stream.IsAudioOnly);
            var file = await _service.DownloadAsync(command);

            if (!File.Exists(file))
            {
                Message = file.Contains("ffmpeg") ? "Ocorreu um erro ao realizar o download do formato selecionado, por favor tente outro formato." : file.ToString();
            }
            else
            {
                string fileUrl = $"/downloads/{Uri.EscapeDataString(Path.GetFileName(file))}";
                await JSRuntime.InvokeVoidAsync("downloadFileFromPath", fileUrl);
                Loading = false;
            }
        }
    }
}
