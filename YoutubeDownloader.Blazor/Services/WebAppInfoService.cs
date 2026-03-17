using YoutubeDownloader.SharedUI.Interfaces;

namespace YoutubeDownloader.Blazor.Services
{
    public class WebAppInfoService : IAppInfoService
    {
        public string Version => "1.0.0-web";
    }
}
