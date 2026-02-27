using YoutubeDownload.Application.ViewModel;

namespace YoutubeDownload.Blazor.Models
{
    public class StreamViewModel
    {
        public string ContainerName { get; set; }
        public string VideoCodec { get; set; }
        public string Resolution { get; set; }
        public double Size { get; set; }
        public bool IsAudioOnly { get; set; }
        public string AudioCodec { get; set; }
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string DisplaySize => Size > 1240 ? $"{Size / 1024:0.##} GB" : $"{Size:0.##} MB";

        public StreamViewModel() { }

        private StreamViewModel(
            string containerName, 
            string videoCodec, 
            string resolution, 
            double size, 
            bool isAudioOnly, 
            string audioCodec, 
            string videoId, 
            string title)
        {
            ContainerName = containerName;
            VideoCodec = videoCodec;
            Resolution = resolution;
            Size = size;
            IsAudioOnly = isAudioOnly;
            AudioCodec = audioCodec;
            VideoId = videoId;
            Title = title;
        }

        public static StreamViewModel Create(StreamInfoViewModel info, StreamManifestViewModel manifest)
            => new(info.ContainerName, info.VideoCodec, info.Resolution, info.Size, info.IsAudioOnly, info.AudioCodec, manifest.VideoId, manifest.Title);
    }
}
