using System.Collections.Generic;

namespace YoutubeDownloader.Models
{
    public static class AvailableOutputFormats
    {
        public static List<string> Audio { get; private set; }
        public static List<string> Video { get; private set; }

        static AvailableOutputFormats()
        {
            Audio = new List<string>
            {
                "MP3",
                "OGG",
                "WAV",
                "WEBM"
            };

            Video = new List<string>
            {
                "MP4",
                "MOV",
                "FLV",
                "AVI"
            };
        }
    }
}
