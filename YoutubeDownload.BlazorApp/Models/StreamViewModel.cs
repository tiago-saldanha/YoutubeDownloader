using YoutubeDownload.Application.ViewModel;

namespace YoutubeDownload.BlazorApp.Models
{
    public readonly record struct StreamViewModel
    {
        public string ContainerName { get; }
        public string VideoCodec { get; }
        public string Resolution { get; }
        public double Size { get; }
        public bool IsAudioOnly { get; }
        public string AudioCodec { get; }
        public string Url { get; }
        public string DisplaySize
        {
            get
            {
                if (Size > 1240)
                {
                    return $"{Size / 1024 :0.##} GB";
                }
                else
                {
                    return $"{Size:0.##} MB";
                }
            }
        }

        private StreamViewModel(string containerName, string videoCodec, string resolution, double size, bool isAudioOnly, string audioCodec, string url)
        {
            ContainerName = containerName;
            VideoCodec = videoCodec;
            Resolution = resolution;
            Size = size;
            IsAudioOnly = isAudioOnly;
            AudioCodec = audioCodec;
            Url = url;
        }

        public static StreamViewModel Create(StreamManifestViewModel stream) => new (stream.ContainerName, stream.VideoCodec, stream.Resolution, stream.Size, stream.IsAudioOnly, stream.AudioCodec, stream.Url);
    }
}
