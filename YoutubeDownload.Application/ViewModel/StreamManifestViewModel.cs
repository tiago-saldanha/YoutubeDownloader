using YoutubeDownload.Application.Extensions;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Application.ViewModel
{
    public record struct StreamManifestViewModel
    {
        public string VideoId { get; private set; }
        public string Title { get; set; }
        public List<StreamInfoViewModel> Streams { get; private set; } = [];

        public static StreamManifestViewModel Create(StreamManifest manifest, Video video) => new(manifest, video);

        private StreamManifestViewModel(StreamManifest manifest, Video video)
        {
            VideoId = video.Id;
            Title = video.Title.FormaterName();
            Streams = manifest.Streams.Select(StreamInfoViewModel.Create).ToList();
        }
    }
}
