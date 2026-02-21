namespace YoutubeDownload.Infrastructure.Interfaces
{
    public interface IFfmpegService
    {
        string Path { get; }
        Task<string> ConvertToMp3(string filePath);
    }
}
