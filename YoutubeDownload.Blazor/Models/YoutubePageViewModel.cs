using YoutubeDownload.Application.Interfaces;

namespace YoutubeDownload.Blazor.Models
{
    public class YoutubePageViewModel
    {
        public string Url { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public bool IsLoading { get; set; }

        public List<StreamViewModel> Streams { get; private set; } = [];

        public async Task SearchAsync(IYoutubeAppService service)
        {
            IsLoading = true;
            Message = "";
            Streams.Clear();

            var manifest = await service.DownloadManifestAsync(Url);

            if (manifest.Streams.Count != 0)
            {
                Streams = [.. manifest.Streams.Select(s => StreamViewModel.Create(s, manifest))];

                Title = manifest.Title;
                Url = "";
            }
            else
            {
                Message = "Nenhum stream encontrado";
            }

            IsLoading = false;
        }
    }
}
