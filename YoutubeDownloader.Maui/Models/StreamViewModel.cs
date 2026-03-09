using YoutubeDownloader.Domain.ViewModel;

namespace YoutubeDownloader.Maui.Models
{
    public readonly record struct StreamViewModel
    {
        public string ContainerName { get; init; }
        public string VideoCodec { get; init; }
        public string Resolution { get; init; }
        public double Size { get; init; }
        public bool IsAudioOnly { get; init; }
        public string AudioCodec { get; init; }
        public string VideoId { get; init; }
        public string Title { get; init; }
        public string DisplaySize => Size > 1240 ? $"{Size / 1024:0.##} GB" : $"{Size:0.##} MB";

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

        public static StreamViewModel Create(StreamInfoViewModel info, string videoId, string title)
            => new(info.ContainerName, info.VideoCodec, info.Resolution, info.Size, info.IsAudioOnly, info.AudioCodec, videoId, title);
    }
}
