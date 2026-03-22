using System.Threading.Channels;

namespace YoutubeDownloader.Web.Middlewares
{
    public class FileCleanupMiddleware(
        RequestDelegate next, 
        Channel<string> channel, 
        ILogger<FileCleanupMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            await next(context);

            if (context.Items.TryGetValue("cleanup-file", out var file))
            {
                var filepath = file as string;

                if (!string.IsNullOrWhiteSpace(filepath))
                {
                    context.Response.OnCompleted(async () =>
                    {
                        logger.LogInformation("OnCompleted dispatch");
                        await channel.Writer.WriteAsync(filepath);
                    });
                }
            }
        }
    }
}
