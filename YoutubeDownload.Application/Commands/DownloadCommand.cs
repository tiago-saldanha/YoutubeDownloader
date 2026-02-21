namespace YoutubeDownload.Application.Commands
{
    public readonly record struct DownloadCommand(string Url, string ContainerName, string? VideoCodec, string? Resolution, string? AudioCodec,  bool IsAudioOnly);
}
