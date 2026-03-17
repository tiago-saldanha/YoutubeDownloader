using MudBlazor.Services;
using YoutubeDownloader.Blazor.Components;
using YoutubeDownloader.Blazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder
    .ConfigureHttpClient()
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

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(YoutubeDownloader.SharedUI.Components.Pages.Home).Assembly);

app.Run();