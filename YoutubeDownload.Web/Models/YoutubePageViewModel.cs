using System.ComponentModel.DataAnnotations;

namespace YoutubeDownload.Web.Models
{
    public class YoutubePageViewModel
    {
        [Required]
        public string Url { get; set; }

        public List<StreamViewModel> Streams { get; set; } = new();

        public bool Loading { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
