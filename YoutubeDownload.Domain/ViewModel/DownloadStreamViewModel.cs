using YoutubeDownload.Domain.Extensions;

namespace YoutubeDownload.Domain.ViewModel
{
    public readonly record struct DownloadStreamViewModel
    {
        public byte[] FileBytes { get; init; }
        public string FileName { get; init; }
        public string ContentType { get; } = "application/octet-stream";

        public DownloadStreamViewModel(byte[] bytes, string fileName)
        {
            FileBytes = bytes;
            FileName = fileName;
        }

        public static DownloadStreamViewModel Create(string filePath, string title, string extension)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException();
            
            var bytes = File.ReadAllBytes(filePath);
            return new DownloadStreamViewModel(bytes, $"{title.FormaterName()}.{extension}");
        }
    }
}
