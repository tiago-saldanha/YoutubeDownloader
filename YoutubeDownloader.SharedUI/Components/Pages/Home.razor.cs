using Microsoft.AspNetCore.Components;
using MudBlazor;
using YoutubeDownloader.Core.Interfaces;
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
        private readonly YoutubePageViewModel _viewModel = new();

        private bool _hasData;
        private string _selectedFormat = "mp4";
        private string _selectedQuality = "best";

        private int _downloadPercent;
        private int _lastReportedPercent = -1;
        private string _statusMessage = string.Empty;

        private string _estimatedSize = "~-- MB";

        private async Task Search(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Snackbar.Add("Paste a valid URL", Severity.Warning);
                return;
            }

            await PrepareSearchAsync(url);
            _cancelationTokenSource = new CancellationTokenSource();

            try
            {
                await _viewModel.SearchAsync(YoutubeAppService, _cancelationTokenSource.Token);

                _hasData = _viewModel.HasResults;

                if (_hasData)
                {
                    CalculateEstimatedSize();
                }
                else
                {
                    Snackbar.Add("No video found for this URL.", Severity.Warning);
                }
            }
            catch (OperationCanceledException)
            {
                Snackbar.Add("Searching canceled.", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
            finally
            {
                await CleanupAsync();
            }
        }

        private async Task PrepareSearchAsync(string url)
        {
            _hasData = false;

            _viewModel.Url = url;
            _viewModel.ClearStreams();

            await InvokeAsync(StateHasChanged);

            _cancelationTokenSource?.Cancel();
            _cancelationTokenSource?.Dispose();

            Snackbar.Add("Searching for video...", Severity.Info);
        }

        private async Task Download()
        {
            if (!_viewModel.HasResults)
                return;

            await PrepareDownloadAsync();
            _cancelationTokenSource = new CancellationTokenSource();

            try
            {
                var command = _viewModel.GetDownloadCommand(_selectedFormat, _selectedQuality);

                var download = await YoutubeAppService.DownloadFileAsync(
                    command, GetProgress(), _cancelationTokenSource.Token);

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
                Snackbar.Add("Download canceled.", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
            finally
            {
                await CleanupAsync();
            }
        }

        private async Task PrepareDownloadAsync()
        {
            Snackbar.Add("Starting download...", Severity.Info);

            await InvokeAsync(StateHasChanged);
        }

        private async Task CleanupAsync()
        {
            _cancelationTokenSource?.Dispose();
            _cancelationTokenSource = null;

            await InvokeAsync(StateHasChanged);
        }

        private void CalculateEstimatedSize()
        {
            if (!_viewModel.HasResults)
                return;

            var stream = _viewModel.SelectBestStream(_selectedFormat, _selectedQuality);

            _estimatedSize = $"~{stream.Size:F1} MB";
        }

        private async Task OnFormatChanged(string format)
        {
            _selectedFormat = format;
            CalculateEstimatedSize();
            await InvokeAsync(StateHasChanged);
        }

        private void OnQualityChanged(string quality)
        {
            _selectedQuality = quality;
            CalculateEstimatedSize();
        }

        private Progress<double> GetProgress()
        {
            _downloadPercent = 0;
            _lastReportedPercent = -1;

            return new Progress<double>(p =>
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
        }

        private void Cancel()
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