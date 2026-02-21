using YoutubeDownload.App.Factory;
using Serilog;

namespace WindowsApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                ApplicationConfiguration.Initialize();
                Application.Run(FactoryApp.Build());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "A aplicação falhou.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}