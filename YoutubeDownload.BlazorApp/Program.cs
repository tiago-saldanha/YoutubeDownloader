using YoutubeDownload.CrossCutting.Extensions;
using YoutubeDownload.BlazorApp.Components;
using YoutubeDownload.BlazorApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ProvideServices();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.ConfigureApp();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
