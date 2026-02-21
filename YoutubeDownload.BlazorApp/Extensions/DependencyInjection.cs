namespace YoutubeDownload.BlazorApp.Extensions
{
    public static class DependencyInjection
    {
        public static WebApplication ConfigureApp(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
            }

            var path = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "downloads");
            Directory.CreateDirectory(path);
            return app;
        }
    }
}
