using System;
using System.Net;
using System.Threading.Tasks;

namespace YoutubeDownloader.Service
{
    public class WebClientService
    {
        public async Task DownloadFileAsyc(Uri downloadUri, string youtubeDlExePath)
        {
            using (WebClient wc = new WebClient())
            {
                await wc.DownloadFileTaskAsync(downloadUri, youtubeDlExePath);
            }
        }
    }
}
