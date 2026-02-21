namespace YoutubeDownload.Application.ViewModel
{
    public record struct DownloadStreamViewModel
    {
        public byte[] FileBytes { get; init; }
        public string FileName { get; init; }
        public bool Success { get; init; }
        public string ContentType { get; } = "application/octet-stream";

        public DownloadStreamViewModel(byte[] bytes, string fileName, bool success)
        {
            FileBytes = bytes;
            FileName = fileName;
            Success = success;
        }

        public static DownloadStreamViewModel Create(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException();
            
            var fileName = Path.GetFileName(filePath);
            var bytes = File.ReadAllBytes(filePath);
            var success = bytes.Length > 0;

            return new DownloadStreamViewModel(bytes, fileName, success);
        }
    }
}
