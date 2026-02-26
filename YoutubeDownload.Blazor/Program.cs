using YoutubeDownload.Blazor.Cache;
using YoutubeDownload.Blazor.Components;
using YoutubeDownload.Blazor.Extensions;
using YoutubeDownload.Blazor.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<DownloadApiClient>(options =>
{
    options.BaseAddress = new Uri(
        builder.Configuration["App:BaseUrl"]!
    );
});

builder.Services.AddControllers();

builder.Services.ConfigureServices();
builder.AddInfraStructure();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<DownloadRequestCache>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();