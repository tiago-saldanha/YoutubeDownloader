using YoutubeDownloader.Domain.Commands;

namespace YoutubeDownloader.Blazor.Services
{
    public class DownloadApiClient(HttpClient client)
    {
        //public async Task<string> PrepareAsync(DownloadCommand command)
        //{
        //    var response = await client.PostAsJsonAsync("api/download/prepare", command);
        //    response.EnsureSuccessStatusCode();
        //    return await response.Content.ReadAsStringAsync();
        //}
    }
}
