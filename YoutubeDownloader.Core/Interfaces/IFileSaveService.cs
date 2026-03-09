namespace YoutubeDownloader.Core.Interfaces
{
    public interface IFileSaveService
    {
        Task SaveAsync(string sourceFilePath, string suggestedFileName);
    }
}
