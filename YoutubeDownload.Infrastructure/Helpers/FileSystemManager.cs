namespace YoutubeDownload.Infrastructure.Helpers
{
    public class FileSystemManager
    {
        private static readonly string OutputDirectory = GetOutputDirectory();

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

        private static string GetOutputDirectory()
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
