namespace YoutubeDownloader.Infrastructure.Interfaces.Ffpmeg
{
    public interface IFfmpegService
    {
        string Path { get; }
        Task<string> ConvertToMp3(string filePath);
    }
}
