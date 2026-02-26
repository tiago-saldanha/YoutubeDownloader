using YoutubeDownload.Application.Commands;
using YoutubeDownload.Blazor.Models;

namespace YoutubeDownload.Blazor.Components.Pages
{
    public partial class Youtube
    {
        private YoutubePageViewModel ViewModel = new();

        private async Task Search()
            => await ViewModel.SearchAsync(Service);

        private async Task Download(StreamViewModel model)
        {
            var command = new DownloadCommand(
                model.VideoId,
                model.Title,
                model.ContainerName,
                model.VideoCodec,
                model.Resolution,
                model.AudioCodec,
                model.IsAudioOnly
            );

            var downloadId = await DownloadClient.PrepareAsync(command);

            Navigation.NavigateTo($"/api/download/{downloadId}", forceLoad: true);
        }
    }
}
