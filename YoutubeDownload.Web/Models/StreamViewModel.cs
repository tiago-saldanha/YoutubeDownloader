namespace YoutubeDownload.Web.Models
{
    public class StreamViewModel
    {
        public string ContainerName { get; set; }
        public string VideoCodec { get; set; }
        public string Resolution { get; set; }
        public double Size { get; set; }
        public bool IsAudioOnly { get; set; }
        public string AudioCodec { get; set; }
        public string Url { get; set; }

        public string DisplaySize =>
            Size > 1240
                ? $"{Size / 1024:0.##} GB"
                : $"{Size:0.##} MB";
    }
}
