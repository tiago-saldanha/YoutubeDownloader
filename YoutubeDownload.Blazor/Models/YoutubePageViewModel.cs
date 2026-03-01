using YoutubeDownload.Application.Interfaces;

namespace YoutubeDownload.Blazor.Models
{
    public class YoutubePageViewModel
    {
        public string Url { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public bool IsLoading { get; set; }
        public List<StreamViewModel> VideoStreams { get; private set; } = [];
        public List<StreamViewModel> AudioStreams { get; private set; } = [];

        public async Task SearchAsync(IYoutubeAppService service)
        {
            IsLoading = true;
            Message = "";

            var manifest = await service.DownloadManifestAsync(Url);

            if (manifest.Streams.Any())
            {
                var streams = manifest.Streams.Select(s => StreamViewModel.Create(s, manifest));

                VideoStreams = [.. streams.Where(s => !s.IsAudioOnly)];
                AudioStreams = [.. streams.Where(s => s.IsAudioOnly)];

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
