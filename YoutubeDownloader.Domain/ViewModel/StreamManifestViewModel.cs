using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Domain.ViewModel
{
    public record struct StreamManifestViewModel
    {
        public string VideoId { get; init; }
        public string Title { get; init; }
        public string ThumbnailUrl { get; init; }
        public IEnumerable<StreamInfoViewModel> Streams { get; private set; } = [];
        
        public static StreamManifestViewModel Create(
            StreamManifest manifest, 
            Video video, 
            string thumbnailUrl) => new(manifest, video, thumbnailUrl);
        
        private StreamManifestViewModel(
            StreamManifest manifest, 
            Video video, 
            string thumbnailUrl)
        {
            VideoId = video.Id;
            Title = video.Title;
            ThumbnailUrl = thumbnailUrl;
            Streams = manifest.Streams
                .OrderByDescending(x => x.Size)
                .OrderByDescending(x => x.Container.Name)
                .OrderByDescending(x => x is IVideoStreamInfo stream ? stream.VideoCodec : x.Container.Name)
                .Select(StreamInfoViewModel.Create);
        }
    }
}
