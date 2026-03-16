using YoutubeDownloader.Core.Interfaces;
using YoutubeDownloader.Domain.Commands;

namespace YoutubeDownloader.SharedUI.Models
{
    public record YoutubePageViewModel
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
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
                ThumbnailUrl = manifest.ThumbnailUrl;
                Url = "";
            }
        }

        public void ClearStreams()
        {
            VideoStreams.Clear();
            AudioStreams.Clear();
            Title = string.Empty;
            ThumbnailUrl = string.Empty;
        }
    }
}
