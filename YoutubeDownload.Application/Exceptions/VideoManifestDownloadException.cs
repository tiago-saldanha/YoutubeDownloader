namespace YoutubeDownload.Application.Exceptions
{
    public sealed class VideoManifestDownloadException : Exception
    {
        public string Url { get; }

        public VideoManifestDownloadException(string url)
            : base($"Error downloading video manifest from URL: {url}")
        {
            Url = url;
        }

        public VideoManifestDownloadException(string url, Exception innerException)
            : base($"Error downloading video manifest from URL: {url}", innerException)
        {
            Url = url;
        }
    }
}
