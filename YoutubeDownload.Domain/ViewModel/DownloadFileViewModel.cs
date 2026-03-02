using YoutubeDownload.Domain.Extensions;

namespace YoutubeDownload.Domain.ViewModel
{
    public readonly record struct DownloadFileViewModel
    {
        public string FilePath { get; init; }
        public string FileName { get; init; }

        public DownloadFileViewModel(string filePath, string fileName)
        {
            FilePath = filePath;
            FileName = fileName;
        }

        public static DownloadFileViewModel Create(string filePath, string title, string extension)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            return new DownloadFileViewModel(filePath, $"{title.FormaterName()}.{extension}");
        }
    }
}
