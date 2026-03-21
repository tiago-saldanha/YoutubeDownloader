using Microsoft.AspNetCore.Components;
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

        private bool _isSearching;
        public bool _hasSearched;
        private int _downloadPercent;
        private string _statusMessage = "";
        private string? _downloadingStreamId;
        private int _lastReportedPercent = -1;

        private bool _isDownloading =>
            !string.IsNullOrEmpty(_downloadingStreamId);

        private string _thumbnailClass =>
            $"mt-4 thumbnail-card {(_viewModel.HasResults ? "thumb-visible" : "thumb-hidden")}";

        private async Task Search()
        {
            _hasSearched = false;
            _isSearching = true;

            Snackbar.Add("Searching for video...", Severity.Info);

            _viewModel.ClearStreams();
            await InvokeAsync(StateHasChanged);
            await _viewModel.SearchAsync(YoutubeAppService);

            _isSearching = false;
            _hasSearched = true;
        }

        private async Task DownloadAsync(StreamViewModel stream)
        {
            _cancelationTokenSource = new CancellationTokenSource();

            Snackbar.Add("Starting download...", Severity.Info);

            _downloadPercent = 0;
            _downloadingStreamId = stream.Id;

            await InvokeAsync(StateHasChanged);

            try
            {
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
                        _statusMessage = $"Downloading {stream.ContainerName} {percent}%";

                        InvokeAsync(StateHasChanged);
                    }
                });

                var download = await YoutubeAppService.DownloadFileAsync(
                    command,
                    progress,
                    _cancelationTokenSource.Token);

                _downloadingStreamId = null;

                if (DeviceService.Desktop)
                {
                    Snackbar.Add("Download completed successfully!", Severity.Success);
                    await DeviceService.OpenFileAsync(download.FilePath);
                }
                else
                {
                    Navigation.NavigateTo($"/api/download/{download.Id}", true);
                    Snackbar.Add("Download ready! Your browser will start the transfer.", Severity.Success);
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
                _downloadingStreamId = null;
                _cancelationTokenSource?.Dispose();
                _cancelationTokenSource = null;

                await InvokeAsync(StateHasChanged);
            }
        }

        private void CancelDownload()
        {
            _cancelationTokenSource?.Cancel();
            Snackbar.Add("Canceling download...", Severity.Warning);
        }

        public void Dispose()
        {
            _cancelationTokenSource?.Cancel();
            _cancelationTokenSource?.Dispose();
        }
    }
}