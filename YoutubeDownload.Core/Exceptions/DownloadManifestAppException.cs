namespace YoutubeDownload.Core.Exceptions
{
    public sealed class DownloadManifestAppException : Exception
    {
        public string Url { get; }

        public DownloadManifestAppException(string url)
            : base($"Error downloading video manifest from URL: {url}")
        {
            Url = url;
        }

        public DownloadManifestAppException(string url, Exception innerException)
            : base($"Error downloading video manifest from URL: {url}", innerException)
        {
            Url = url;
        }
    }
}
