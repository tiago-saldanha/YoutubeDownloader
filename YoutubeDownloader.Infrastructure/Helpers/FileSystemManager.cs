namespace YoutubeDownloader.Infrastructure.Helpers
{
    public class FileSystemManager
    {
        public static readonly string OutputDirectory = CreateOutputDirectory();

        public static string CreateFile(string extension)
        {
            var filePath = Path.Combine(OutputDirectory, $"{Guid.NewGuid()}.{extension}");
            FileSystemManager.RemoveFile(filePath);
            return filePath;
        }
        
        public static void RemoveFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static string CreateOutputDirectory()
        {
            var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "downloads");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            return outputDirectory;
        }
    }
}
