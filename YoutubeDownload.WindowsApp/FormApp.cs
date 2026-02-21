using YoutubeExplode.Common;
using YoutubeDownload.App.Controlls;
using YoutubeDownload.Application.Commands;
using YoutubeDownload.Application.Interfaces;

namespace WindowsApp
{
    public partial class FormApp : Form
    {
        private readonly IYoutubeService _service;

        public FormApp(IYoutubeService service)
        {
            InitializeComponent();
            _service = service;
        }

        private async void DownloadManifestInfo(object sender, EventArgs e)
        {
            Ok(false);
            if (string.IsNullOrEmpty(textBoxUrlVideo.Text))
            {
                MessageBox.Show($"Informe uma url válida!");
                return;
            }

            try
            {
                ChangeEnableButtons();
                var streams = await _service.DownloadManifest(textBoxUrlVideo.Text);
                if (streams != null && streams.Any())
                {
                    dataGridView.DataSource = null;
                    dataGridView.DataSource = streams.Select(stream => new
                    {
                        stream.ContainerName,
                        MegaBytes = Math.Round(stream.Size, 2),
                        Resolution = stream.Resolution ?? null,
                        VideoCodec = stream.VideoCodec ?? null,
                        IsAudioOnly = stream.AudioCodec != null,
                        AudioCodec = stream.AudioCodec ?? null,
                        Url = stream.Url ?? textBoxUrlVideo.Text
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao baixar informações do manifest: {ex.Message}");
            }
            finally
            {
                ChangeEnableButtons();
                textBoxUrlVideo.Clear();
            }
        }

        private void DownloadManifestInfoEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DownloadManifestInfo(sender, e);
            }
        }

        private async void Download(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var row = dataGridView.SelectedRows[0];
                var file = string.Empty;
                try
                {
                    ChangeEnableButtons();
                    var command = GetCommand(row);
                    file = await _service.Download(command);
                    textBoxOutput.Clear();
                }
                catch
                {
                }
                finally
                {
                    textBoxOutput.Text = file;

                    if (!File.Exists(file)) MessageBox.Show($"Erro ao baixar o arquivo: {file}");
                    textBoxUrlVideo.Clear();
                    if (checkBoxAutoPlay.Checked && !string.IsNullOrEmpty(file)) Play(file);
                    ChangeEnableButtons();
                    Ok(true);
                    if (checkBoxConverterMp3.Checked) Converter(file);
                }
            }
        }

        private DownloadCommand GetCommand(DataGridViewRow row)
        {
            bool.TryParse(row.Cells["IsAudioOnly"].Value.ToString(), out bool isAudioOnly);
            var containerName = row.Cells["ContainerName"].Value.ToString();
            var audioCodec = row.Cells["AudioCodec"].Value?.ToString();
            var resolution = row.Cells["Resolution"].Value?.ToString();
            var videoCodec = row.Cells["VideoCodec"].Value?.ToString();
            var url = row.Cells["Url"].Value.ToString();
            return new DownloadCommand(url, containerName, videoCodec, resolution, audioCodec, isAudioOnly);
        }

        private void Play(object sender, EventArgs e)
        {
            ChangeEnableButtons();

            if (string.IsNullOrEmpty(textBoxOutput.Text))
            {
                MessageBox.Show($"Nenhum arquivo lozalido!");
                return;
            }
            else
            {
                Play(textBoxOutput.Text);
            }

            ChangeEnableButtons();
        }

        private void Play(string file) => MediaPlayer.Play(file);

        private async void Converter(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show($"O arquivo({file}) não existe.");
                return;
            }

            await _service.Converter(file);
        }

        private void ChangeEnableButtons()
        {
            buttonPlay.Enabled = !buttonPlay.Enabled;
            buttonDownloadInfo.Enabled = !buttonDownloadInfo.Enabled;
            buttonDownload.Enabled = !buttonDownload.Enabled;
        }

        private void Atualizar(object sender, EventArgs e)
        {
            textBoxUrlVideo.Clear();
            textBoxOutput.Clear();
            dataGridView.DataSource = null;
            Ok(false);
        }

        private void Ok(bool valid) => buttonOk.Image = valid ? Image.FromFile(Path.Combine(Environment.CurrentDirectory, "icons", "verificar-verde.png")) : Image.FromFile(Path.Combine(Environment.CurrentDirectory, "icons", "verificar-cinza.png"));
    }
}
