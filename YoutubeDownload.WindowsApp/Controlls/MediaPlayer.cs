using System.Diagnostics;

namespace YoutubeDownload.App.Controlls
{
    public static class MediaPlayer
    {
        public static void Play(string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists) MessageBox.Show($"Arquivo inválido!");

            if (file.Extension == ".webm" || file.Extension == ".mp4" || file.Extension == ".mp3")
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = file.FullName,
                            UseShellExecute = true,
                            CreateNoWindow = false
                        }
                    };

                    process.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao tentar reproduzir a mídia: {ex.Message}");
                }
            }
        }
    }
}
