using System;
using System.IO;
using System.Threading.Tasks;

namespace YoutubeDownloader.Service
{
    public class ApplicationService
    {
        private readonly YoutubeDlWrapperService _youtubeDlWrapperService;
        private readonly WebClientService _webClientService;

        public ApplicationService(YoutubeDlWrapperService youtubeDlWrapperService, WebClientService webClientService)
        {
            _youtubeDlWrapperService = youtubeDlWrapperService;
            _webClientService = webClientService;
        }

        public void DownloadYoutubeDlIfNotExists()
        {
            if (!File.Exists(_youtubeDlWrapperService.YoutubeDlExePath))
            {
                Task.WaitAll(UpdateYoutubeDl());
            }
        }

        public async Task UpdateYoutubeDl()
        {
            CreateFolder(_youtubeDlWrapperService.YoutubeDlDirectoryPath);

            Uri downloadUri = new Uri("https://yt-dl.org/latest/youtube-dl.exe");
            await _webClientService.DownloadFileAsyc(downloadUri, _youtubeDlWrapperService.YoutubeDlExePath);
        }

        private void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        //TODO: Add FFMpeg auto updater:
        //https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.7z
        //https://www.gyan.dev/ffmpeg/builds/
    }
}
