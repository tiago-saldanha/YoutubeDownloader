namespace YoutubeDownloader.SharedUI.Interfaces
{
    public interface IDeviceService
    {
        bool Desktop { get; }
        Task OpenFileAsync(string filePath);
    }
}
