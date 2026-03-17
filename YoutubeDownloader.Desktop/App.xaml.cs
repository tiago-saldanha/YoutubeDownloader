namespace YoutubeDownloader.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage())
            {
                Title = "Youtube Downloader",

                Width = 1152,
                Height = 864,

                MinimumWidth = 800,
                MinimumHeight = 600
            };

            return window;
        }
    }
}