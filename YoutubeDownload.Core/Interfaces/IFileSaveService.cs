namespace YoutubeDownload.Core.Interfaces
{
    public interface IFileSaveService
    {
        Task SaveAsync(string sourceFilePath, string suggestedFileName);
    }
}
