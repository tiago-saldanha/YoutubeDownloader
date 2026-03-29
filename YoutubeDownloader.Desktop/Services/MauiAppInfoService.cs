using YoutubeDownloader.SharedUI.Interfaces;

namespace YoutubeDownloader.Desktop.Services
{
    public class MauiAppInfoService : IAppInfoService
    {
        public string Version => AppInfo.Current.VersionString;
    }
}
