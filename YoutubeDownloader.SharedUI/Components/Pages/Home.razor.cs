using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using YoutubeDownloader.Core.Interfaces;
using YoutubeDownloader.Domain.Commands;
using YoutubeDownloader.SharedUI.Interfaces;
using YoutubeDownloader.SharedUI.Models;

namespace YoutubeDownloader.SharedUI.Components.Pages
{
    public partial class Home : ComponentBase, IDisposable
    {
        [Inject] public IYoutubeAppService YoutubeAppService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IDeviceService DeviceService { get; set; } = default!;
        [Inject] public ISnackbar Snackbar { get; set; } = default!;

        private CancellationTokenSource? _cancelationTokenSource;

        private YoutubePageViewModel _viewModel = new();

        private string _url = "";
        private bool _isSearching;
        public bool _hasSearched;
        private bool _hasData;

        private readonly string _mp4 = "mp4";
        private readonly string _mp3 = "mp3";
        private readonly string _best = "best";

        private readonly List<string> _videoQuality = [ "1080", "720", "480" ];

        private string _selectedFormat = "mp4";
        private string _selectedQuality = "best";

        private int _downloadPercent;
        private int _lastReportedPercent = -1;
        private string _statusMessage = "";

        private bool _isDownloading;

        private string _estimatedSize = "~-- MB";

        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(_url))
            {
                Snackbar.Add("Paste a valid URL", Severity.Warning);
                return;
            }

            _hasSearched = false;
            _isSearching = true;
            _hasData = false;

            Snackbar.Add("Searching for video...", Severity.Info);

            _viewModel.Url = _url;
            _viewModel.ClearStreams();

            await InvokeAsync(StateHasChanged);

            await _viewModel.SearchAsync(YoutubeAppService);

            _hasSearched = true;
            _isSearching = false;
            _hasData = _viewModel.HasResults;

            CalculateEstimatedSize();

            await InvokeAsync(StateHasChanged);
        }

        private async Task Download()
        {
            if (!_viewModel.HasResults)
                return;

            _cancelationTokenSource = new CancellationTokenSource();

            _isDownloading = true;
            _downloadPercent = 0;
            _lastReportedPercent = -1;

            Snackbar.Add("Starting download...", Severity.Info);

            await InvokeAsync(StateHasChanged);

            try
            {
                var stream = SelectBestStream();

                var command = new DownloadCommand(
                    stream.VideoId,
                    stream.Title,
                    stream.ContainerName,
                    stream.VideoCodec,
                    stream.Resolution,
                    stream.AudioCodec,
                    stream.IsAudioOnly
                );

                var progress = new Progress<double>(p =>
                {
                    var percent = (int)Math.Floor(p * 100);

                    if (percent != _lastReportedPercent)
                    {
                        _lastReportedPercent = percent;
                        _downloadPercent = percent;
                        _statusMessage = $"Downloading {_selectedFormat} {percent}%";

                        InvokeAsync(StateHasChanged);
                    }
                });

                var download = await YoutubeAppService.DownloadFileAsync(
                    command,
                    progress,
                    _cancelationTokenSource.Token);

                _isDownloading = false;

                if (DeviceService.Desktop)
                {
                    Snackbar.Add("Download completed!", Severity.Success);
                    await DeviceService.OpenFileAsync(download.FilePath);
                }
                else
                {
                    Navigation.NavigateTo($"/api/download/{download.Id}", true);
                    Snackbar.Add("Download ready!", Severity.Success);
                }
            }
            catch (OperationCanceledException)
            {
                Snackbar.Add("Download canceled.", Severity.Warning);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isDownloading = false;
                _cancelationTokenSource?.Dispose();
                _cancelationTokenSource = null;

                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !_isSearching)
            {
                await Search();
            }
        }

        private StreamViewModel SelectBestStream()
        {
            if (_selectedFormat == _mp3)
            {
                return _viewModel.AudioStreams
                    .OrderByDescending(s => s.Size)
                    .First();
            }

            var candidates = _viewModel.VideoStreams
                .Where(s => s.ContainerName == _mp4);

            if (_selectedQuality != _best)
            {
                candidates = candidates
                    .Where(s => s.Resolution.Contains(_selectedQuality));
            }

            var best = candidates
                .Where(s => s.VideoCodec.StartsWith("avc1"))
                .OrderByDescending(s => s.Size)
                .FirstOrDefault();

            if (best == null)
            {
                best = candidates
                    .OrderByDescending(s => s.Size)
                    .First();
            }

            return best;
        }

        private void CalculateEstimatedSize()
        {
            if (!_viewModel.HasResults)
                return;

            var stream = SelectBestStream();

            var sizeMb = stream.Size;

            _estimatedSize = $"~{sizeMb:F1} MB";
        }

        private void OnFormatChanged(string format)
        {
            _selectedFormat = format;
            CalculateEstimatedSize();
        }

        private void OnQualityChanged(string quality)
        {
            _selectedQuality = quality;
            CalculateEstimatedSize();
        }

        private void CancelDownload()
        {
            _cancelationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            _cancelationTokenSource?.Cancel();
            _cancelationTokenSource?.Dispose();
        }
    }
}