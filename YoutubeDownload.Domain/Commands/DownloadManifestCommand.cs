namespace YoutubeDownload.Domain.Commands
{
    public readonly record struct DownloadManifestCommand(
        string Url);
}
