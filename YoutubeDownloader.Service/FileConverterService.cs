using FFMpegCore;
using System.IO;
using System.Threading.Tasks;

namespace YoutubeDownloader.Service
{
    public class FileConverterService
    {
        public delegate void ConsoleOutputReceivedEventHandler(object sender, string arg);
        public event ConsoleOutputReceivedEventHandler ConsoleOutputReceived;

        public async Task ConvertFiles(string inputDirectory, string outputDirectory, string outputFormat, System.Threading.CancellationTokenSource cancellationToken)
        {
            string[] inputFiles = Directory.GetFiles(inputDirectory);

            foreach (var filePath in inputFiles)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException("Convert process is manually aborted.");
                }

                string outputFileName = Path.GetFileNameWithoutExtension(filePath);
                string outputFilePath = $"{outputDirectory}{Path.DirectorySeparatorChar}{outputFileName}.{outputFormat}";
                await ConvertFile(filePath, outputFilePath, outputFormat)
                    .ContinueWith(x => DeleteSourceFile(filePath))
                    .ContinueWith(x => ConsoleOutputReceived?.Invoke(this, $"[Convert] {outputFileName} was converted."));
            }
        }

        private async Task ConvertFile(string inputFilePath, string outputFilePath, string outputFormat)
        {
            //TODO: Move to ApplicationService
            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = "./Resources", TemporaryFilesFolder = "/tmp" });

            await FFMpegArguments
                    .FromFileInput(inputFilePath)
                    .OutputToFile(outputFilePath, false, options => options
                        .ForceFormat(outputFormat))
                    .ProcessAsynchronously();
        }

        private void DeleteSourceFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
