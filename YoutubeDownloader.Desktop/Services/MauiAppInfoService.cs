using YoutubeDownloader.SharedUI.Interfaces;

namespace YoutubeDownloader.Desktop.Services
{
    public class MauiAppInfoService : IAppInfoService
    {
        public string Version
        {
            get
            {
                var version = AppInfo.Current.VersionString;
                return version;
            }
        }
    }
}
