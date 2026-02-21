namespace YoutubeDownload.Application.Exceptions
{
    public sealed class VideoDownloadException : Exception
    {
        public string VideoId { get; }
        public string Title { get; }

        public VideoDownloadException(string videoId, string title)
            : base($"Error downloading video '{title}' (ID: {videoId}).")
        {
            VideoId = videoId;
            Title = title;
        }

        public VideoDownloadException(string videoId, string title, Exception innerException)
            : base($"Error downloading video '{title}' (ID: {videoId}).", innerException)
        {
            VideoId = videoId;
            Title = title;
        }
    }
}
