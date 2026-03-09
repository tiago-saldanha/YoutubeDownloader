using YoutubeDownloader.Domain.Extensions;

namespace YoutubeDownloader.Domain.ViewModel
{
    public readonly record struct DownloadFileViewModel
    {
        public string Id { get; init; }
        public string FilePath { get; init; }
        public string FileName { get; init; }
        public string ContentType { get; } = "application/octet-stream";

        public DownloadFileViewModel(string id, string filePath, string fileName)
        {
            Id = id;
            FilePath = filePath;
            FileName = fileName;
        }

        public static DownloadFileViewModel Create(string filePath, string title, string extension)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            return new DownloadFileViewModel(Guid.NewGuid().ToString(), filePath, $"{title.FormaterName()}.{extension}");
        }
    }
}
