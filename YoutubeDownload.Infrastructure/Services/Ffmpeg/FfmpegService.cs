using System.Diagnostics;
using Microsoft.Extensions.Options;
using YoutubeDownload.Infrastructure.Configuration;
using YoutubeDownload.Infrastructure.Interfaces;

namespace YoutubeDownload.Infrastructure.Services.Ffmpeg
{
    public class FfmpegService : IFfmpegService
    {
        private readonly string _ffmpegPath;

        public FfmpegService(IOptions<FfmpegOptions> options)
        {
            var path = options.Value.Path ?? string.Empty;

            _ffmpegPath = System.IO.Path.IsPathRooted(path)
                ? path
                : System.IO.Path.Combine(AppContext.BaseDirectory, path);

            if (!File.Exists(_ffmpegPath))
            {
                throw new FileNotFoundException(
                    $"FFmpeg not found in '{_ffmpegPath}'. " +
                    "See appsettings.json file if binary exists.");
            }
        }

        public string Path => _ffmpegPath;

        public async Task<string> ConvertToMp3(string file)
        {
            var output = System.IO.Path.ChangeExtension(file, ".mp3");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-i \"{file}\" -preset ultrafast -b:a 192k \"{output}\" -y",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();
            return output;
        }

        private async Task<string> ConvertToMp4(string file)
        {
            var threads = Math.Max(1, Environment.ProcessorCount - 2);
            var output = System.IO.Path.ChangeExtension(file, ".mp4");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-i \"{file}\" -c:v libx264 -preset ultrafast -c:a aac -b:a 128k -threads {threads} -y \"{output}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            return output;
        }
    }
}
