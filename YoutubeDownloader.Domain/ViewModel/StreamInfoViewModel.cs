using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Domain.ViewModel
{
    public record struct StreamInfoViewModel
    {
        public string ContainerName { get; private set; }
        public double Size { get; private set; }
        public bool IsAudioOnly { get; private set; }
        public string AudioCodec { get; private set; }
        public string Resolution { get; private set; }
        public string VideoCodec { get; private set; }

        public static StreamInfoViewModel Create(IStreamInfo stream) => new(stream);

        private StreamInfoViewModel(IStreamInfo stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            ContainerName = stream.Container.Name;
            Size = stream.Size.MegaBytes;

            AddMediaDetails(stream);
        }

        private void AddMediaDetails(IStreamInfo stream)
        {
            if (stream is IAudioStreamInfo audio)
            {
                AudioCodec = audio.AudioCodec;
                IsAudioOnly = true;
            }
            else if (stream is IVideoStreamInfo video)
            {
                Resolution = video.VideoQuality.Label;
                VideoCodec = video.VideoCodec;
            }
        }
    }
}
