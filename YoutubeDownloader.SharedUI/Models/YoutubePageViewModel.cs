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

        private readonly string _mp4 = "mp4";
        private readonly string _mp3 = "mp3";
        private readonly string _best = "best";

        public async Task SearchAsync(
            IYoutubeAppService service, 
            CancellationToken cancellationToken)
        {
            var command = new DownloadManifestCommand(Url);
            var manifest = await service.DownloadManifestAsync(command, cancellationToken);

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

        public bool HasResults => VideoStreams.Count != 0 || AudioStreams.Count != 0;

        public void ClearStreams()
        {
            VideoStreams.Clear();
            AudioStreams.Clear();
            Title = string.Empty;
            ThumbnailUrl = string.Empty;
        }

        public StreamViewModel SelectBestStream(string format, string quality)
        {
            if (format == _mp3)
            {
                return AudioStreams
                    .OrderByDescending(s => s.Size)
                    .First();
            }

            var candidates = VideoStreams
                .Where(s => s.ContainerName == _mp4);

            if (quality != _best)
            {
                candidates = candidates
                    .Where(s => s.Resolution.Contains(quality));
            }

            var best = candidates
                .Where(s => s.VideoCodec.StartsWith("avc1"))
                .OrderByDescending(s => s.Size)
                .FirstOrDefault();

            return best;
        }

        public DownloadCommand GetDownloadCommand(string format, string quality)
        {
            var stream = SelectBestStream(format, quality);

            return new DownloadCommand(
                stream.VideoId,
                stream.Title,
                stream.ContainerName,
                stream.VideoCodec,
                stream.Resolution,
                stream.AudioCodec,
                stream.IsAudioOnly
            );
        }
    }
}
