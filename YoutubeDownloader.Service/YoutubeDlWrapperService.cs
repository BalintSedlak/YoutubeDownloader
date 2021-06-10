using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace YoutubeDownloader.Service
{
    public class YoutubeDlWrapperService
    {
        public delegate void OutputReceivedEventHandler(object sender, string arg);
        public event OutputReceivedEventHandler OutputReceived;

        public string YoutubeDlDirectoryPath => $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}YoutubeDl";
        public string YoutubeDlExePath => $"{YoutubeDlDirectoryPath}{Path.DirectorySeparatorChar}youtube-dl.exe";

        private Process _process; 

        /// <summary>
        /// Based on https://stackoverflow.com/questions/10788982/is-there-any-async-equivalent-of-process-start
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public async Task<int> ExecuteCommand(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = arguments;
            startInfo.FileName = YoutubeDlExePath;

            try
            {
                using (_process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
                {
                    return await RunProcessAsync(_process).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                //todo
                // Log error.
                throw;
            }
        }

        /// <summary>
        /// Based on https://stackoverflow.com/questions/10788982/is-there-any-async-equivalent-of-process-start
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private Task<int> RunProcessAsync(Process process)
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

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutputReceived.Invoke(sender, e.Data);
        }

        public void KillProcess()
        {
            _process.Kill();
        }
    }
}
