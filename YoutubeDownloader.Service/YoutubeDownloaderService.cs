using System.IO;
using System.Threading.Tasks;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Service
{
    public class YoutubeDownloaderService
    {
        private readonly YoutubeDlWrapperService _youtubeDlWrapperService;

        public delegate void ConsoleOutputReceivedEventHandler(object sender, string arg);
        public event ConsoleOutputReceivedEventHandler ConsoleOutputReceived;

        private const string _quote = "\"";

        public YoutubeDownloaderService(YoutubeDlWrapperService youtubeDlWrapperService)
        {
            _youtubeDlWrapperService = youtubeDlWrapperService;
            _youtubeDlWrapperService.OutputReceived += OutputReceivedFromYoutubeDl;
        }

        private void OutputReceivedFromYoutubeDl(object sender, string arg)
        {
            ConsoleOutputReceived.Invoke(sender, arg);
        }

        public async Task DownloadPlaylistAsync(string url, string saveLocation, QualitySetting selectedQualitySetting, FormatSetting selectedFormatSetting)
        {
            string argument = AssembleArguments(url, saveLocation, includePlaylist: true, selectedQualitySetting, selectedFormatSetting);
            await _youtubeDlWrapperService.ExecuteCommand(argument);
        }

        public async Task DownloadSingleFileAsync(string url, string saveLocation, QualitySetting selectedQualitySetting, FormatSetting selectedFormatSetting)
        {
            string argument = AssembleArguments(url, saveLocation, includePlaylist: false, selectedQualitySetting, selectedFormatSetting);
            await _youtubeDlWrapperService.ExecuteCommand(argument);
        }

        public async Task ListFormatsForSingleFileAsync(string url)
        {
            string arguments = $"-F {url}";
            await _youtubeDlWrapperService.ExecuteCommand(arguments);
        }

        private string AssembleArguments(string url, string saveLocation, bool includePlaylist, QualitySetting selectedQualitySetting, FormatSetting selectedFormatSetting)
        {
            string playlist = string.Empty;

            if (includePlaylist)
            {
                playlist = "--yes-playlist";
            }
            else
            {
                playlist = "--no-playlist";
            }

            string quality = selectedQualitySetting.ToString().ToLower();
            string format = string.Empty;

            switch (selectedFormatSetting)
            {
                case FormatSetting.VideoAndAudio:
                    //Todo: Change this case to the following:
                    //youtube-dl -f 'bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio' --merge-output-format mp4 "link to youtube video"
                    break;
                case FormatSetting.Audio:
                    format = FormatSetting.Audio.ToString().ToLower();
                    break;
                case FormatSetting.Video:
                    format = FormatSetting.Video.ToString().ToLower();
                    break;
            }

            string finalArgument = $"-f {quality}{format} {playlist} -o {_quote}{saveLocation}{Path.DirectorySeparatorChar}%(title)s.%(ext)s{_quote} {url}";

            return finalArgument;
        }

        public void AbortDownload()
        {
            _youtubeDlWrapperService.KillProcess();
        }
    }
}
