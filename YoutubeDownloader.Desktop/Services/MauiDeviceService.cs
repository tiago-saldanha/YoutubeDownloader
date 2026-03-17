using YoutubeDownloader.SharedUI.Interfaces;

namespace YoutubeDownloader.Desktop.Services
{
    public class MauiDeviceService : IDeviceService
    {
        public bool Desktop => true;

        public async Task OpenFileAsync(string filePath)
        {
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
    }
}
