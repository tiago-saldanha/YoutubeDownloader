using YoutubeDownload.Core.Interfaces;
using YoutubeDownload.Domain.Commands;

namespace YoutubeDownload.Blazor.Models
{
    public record YoutubePageViewModel
    {
        public string Url { get; set; } = "";
        public string Title { get; set; } = "";
        public List<StreamViewModel> VideoStreams { get; private set; } = [];
        public List<StreamViewModel> AudioStreams { get; private set; } = [];

        public async Task SearchAsync(IYoutubeAppService service)
        {
            var command = new DownloadManifestCommand(Url);
            var manifest = await service.DownloadManifestAsync(command);

            if (manifest.Streams.Any())
            {
                var streams = manifest.Streams.Select(s => StreamViewModel.Create(s, manifest.VideoId, manifest.Title));

                VideoStreams = [.. streams.Where(s => !s.IsAudioOnly)];
                AudioStreams = [.. streams.Where(s => s.IsAudioOnly)];

                Title = manifest.Title;
                Url = "";
            }
        }

        public void ClearStreams()
        {
            VideoStreams.Clear();
            AudioStreams.Clear();
        }
    }
}
