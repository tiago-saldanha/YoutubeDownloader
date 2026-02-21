using System.Text.RegularExpressions;

namespace YoutubeDownload.Application.Extensions
{
    public static class StringExtensions
    {
        public static string FormaterName(this string input) => Regex.Replace(input, @"[^a-zA-Z0-9\s\.\-\u00C0-\u00FF]", "");
    }
}
