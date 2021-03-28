using System.ComponentModel;

namespace YoutubeDownloader.Models
{
    public enum QualitySetting
    {
        [Description("Highest quality")] Best,
        [Description("Lowest quality")] Worst,
    }
}
