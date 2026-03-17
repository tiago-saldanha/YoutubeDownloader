using YoutubeDownloader.SharedUI.Interfaces;

namespace YoutubeDownloader.Blazor.Services
{
    public class WebDeviceService : IDeviceService
    {
        public bool Desktop => false;

        public Task OpenFileAsync(string filePath) => Task.CompletedTask;
    }
}
