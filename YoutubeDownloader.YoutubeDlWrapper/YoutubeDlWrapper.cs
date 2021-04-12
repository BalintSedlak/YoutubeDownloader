using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace YoutubeDownloader.YoutubeDlWrapper
{
    public static class YoutubeDlWrapper
    {
        public delegate void OutputReceivedEventHandler(object sender, string arg);

        private static string YoutubeDlPath { get; set; }

        public static event OutputReceivedEventHandler OutputReceived;

        static YoutubeDlWrapper()
        {
            YoutubeDlPath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}YoutubeDl{Path.DirectorySeparatorChar}youtube-dl.exe";
        }

        //todo: paramteters
        /// <summary>
        /// Based on https://stackoverflow.com/questions/10788982/is-there-any-async-equivalent-of-process-start
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteCommand(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = arguments;
            startInfo.FileName = YoutubeDlPath;

            try
            {
                using (Process process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
                {
                    return await RunProcessAsync(process).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                //todo
                // Log error.
            }

            return -1;
        }

        /// <summary>
        /// Based on https://stackoverflow.com/questions/10788982/is-there-any-async-equivalent-of-process-start
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private static Task<int> RunProcessAsync(Process process)
        {
            var task = new TaskCompletionSource<int>();

            process.Exited += (s, ea) => task.SetResult(process.ExitCode);
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_OutputDataReceived;

            bool started = process.Start();
            if (!started)
            {
                throw new InvalidOperationException("Could not start process: " + process);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return task.Task;
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutputReceived.Invoke(sender, e.Data);
        }
    }
}
