using System.Diagnostics;

namespace YoutubeDownload.Application.Services
{
    public static class FfmpegService
    {
        public static string Path = Get();

        private static string Get()
        {
            var path = string.Empty;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
                path = "/app/lib/ffmpeg";
            else
                path = System.IO.Path.Combine(AppContext.BaseDirectory, "lib", "ffmpeg.exe");
            
            if (!File.Exists(path))
                throw new FileNotFoundException("FFmpeg não encontrado. Certifique-se de que o ffmpeg.exe/ffmpeg está na pasta lib.");

            return path;
        }

        public static string ConvertToMp3(string file)
        {
            var output = System.IO.Path.ChangeExtension(file, ".mp3");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path,
                    Arguments = $"-i \"{file}\" -preset ultrafast -b:a 192k \"{output}\" -y",
                    UseShellExecute = true,
                    CreateNoWindow = false
                }
            };

            process.Start();
            process.WaitForExit();

            return output;
        }

        private static string ConvertToMp4(string file)
        {
            var threads = Math.Max(1, Environment.ProcessorCount - 2);
            var output = System.IO.Path.ChangeExtension(file, ".mp4");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path,
                    Arguments = $"-i \"{file}\" -c:v libx264 -preset ultrafast -c:a aac -b:a 128k -threads {threads} -y \"{output}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                }
            };

            process.Start();
            process.WaitForExit();

            return output;
        }
    }
}