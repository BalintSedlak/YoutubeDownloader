using System.ComponentModel;

namespace YoutubeDownloader.Models
{
    public enum FormatSetting
    {
        [Description("Video and Audio")] VideoAndAudio,
        [Description("Audio only")] Audio,
        [Description("Video only")] Video
    }
}
