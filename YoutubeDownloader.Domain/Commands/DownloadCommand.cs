namespace YoutubeDownloader.Domain.Commands
{
    public readonly record struct DownloadCommand(
        string VideoId, 
        string Title, 
        string ContainerName, 
        string VideoCodec, 
        string Resolution, 
        string AudioCodec,  
        bool IsAudioOnly);
}
