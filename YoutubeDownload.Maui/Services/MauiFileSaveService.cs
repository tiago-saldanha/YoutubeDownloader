using YoutubeDownload.Core.Interfaces;

namespace YoutubeDownload.Maui.Services
{
    public class MauiFileSaveService : IFileSaveService
    {
        public async Task SaveAsync(string sourceFilePath, string suggestedFileName)
        {
            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException("Source file not found.", sourceFilePath);

            var result = await FilePicker.Default.PickAsync(
                new PickOptions
                {
                    PickerTitle = suggestedFileName,
                });

            if (result is null)
                return;

            await using var source = File.OpenRead(sourceFilePath);
            await using var destination = File.Create(result.FullPath);

            await source.CopyToAsync(destination);
        }
    }
}
