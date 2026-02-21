using YoutubeExplode.Videos.Streams;

namespace YoutubeDownload.Application.ViewModel
{
    public class StreamManifestViewModel
    {
        public string Url { get; set; }
        public string ContainerName { get; private set; }
        public double Size { get; private set; }
        public bool IsAudioOnly { get; private set; }
        public string AudioCodec { get; private set; }
        public string Resolution { get; private set; }
        public string VideoCodec { get; private set; }

        public static StreamManifestViewModel Create(IStreamInfo stream, string url) => new(stream, url);

        private StreamManifestViewModel(IStreamInfo stream, string url)
        {
            ArgumentNullException.ThrowIfNull(stream);

            Url = url;
            ContainerName = stream.Container.Name;
            Size = stream.Size.MegaBytes;
            IsAudioOnly = stream is IAudioStreamInfo;

            AddMedia(stream);
        }

        private void AddMedia(IStreamInfo stream)
        {
            if (stream is IAudioStreamInfo audioStream)
            {
                AudioCodec = audioStream.AudioCodec;
            }
            else if (stream is IVideoStreamInfo videoStream)
            {
                Resolution = videoStream.VideoQuality.Label;
                VideoCodec = videoStream.VideoCodec;
            }
        }
    }
}
