using MudBlazor.Services;
using YoutubeDownloader.Web.Components;
using YoutubeDownloader.Web.Extensions;
using YoutubeDownloader.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder
    .ConfigureWeb()
    .ConfigureCache()
    .ConfigureApplication()
    .ConfigureInfrastructure();

builder.Services.AddMudServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseMiddleware<FileCleanupMiddleware>();

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(YoutubeDownloader.SharedUI.Components.Pages.Home).Assembly);

app.Run();