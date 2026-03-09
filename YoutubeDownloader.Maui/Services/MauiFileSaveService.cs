using CommunityToolkit.Maui.Storage;
using YoutubeDownloader.Core.Interfaces;

namespace YoutubeDownloader.Maui.Services
{
    public class MauiFileSaveService : IFileSaveService
    {
        public async Task SaveAsync(string sourceFilePath, string suggestedFileName)
        {
            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException(sourceFilePath);

            using var source = File.OpenRead(sourceFilePath);

            var downloads = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );

            var result = await FileSaver.SaveAsync(
                downloads,
                suggestedFileName,
                source
            );

            if (result.Exception != null)
                throw result.Exception;
        }
    }
}
