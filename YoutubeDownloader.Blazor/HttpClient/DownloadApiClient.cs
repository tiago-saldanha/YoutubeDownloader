using YoutubeDownloader.Domain.Commands;

namespace YoutubeDownloader.Blazor.Http
{
    public class DownloadApiClient(HttpClient client)
    {
        public async Task<string> PrepareAsync(DownloadCommand command)
        {
            var response = await client.PostAsJsonAsync("api/download/prepare", command);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
