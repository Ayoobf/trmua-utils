using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace trmua_utils.utils
{
    public class RotateFile
    {
        private CancellationTokenSource _cts;
        public event Action<string> LogMessage;
        private bool _exitLoop = false;

        public async Task RotateFilesAsync(string folderPath, int subfolderToBeLeftOut, IProgress<int> progress, Dispatcher dispatcher)
        {
            _cts = new CancellationTokenSource();
            _exitLoop = false;
            try
            {
                var files = Directory.GetFiles(folderPath);
                int numOfFiles = files.Length - subfolderToBeLeftOut;
                LogMessage?.Invoke($"Found {numOfFiles} files to rotate");

                for (int i = 0; i < numOfFiles; i++)
                {
                    if (_exitLoop || _cts.Token.IsCancellationRequested)
                    {
                        LogMessage?.Invoke("Operation cancelled");
                        break;
                    }

                    string currentFile = files[i];
                    LogMessage?.Invoke($"Rotating file {i + 1} of {numOfFiles}: {Path.GetFileName(currentFile)}");

                    await dispatcher.InvokeAsync(() =>
                    {
                        Thread.Sleep(1000);
                        SendKeys.SendWait("^."); // Ctrl + .
                        Thread.Sleep(1000); // Short delay after rotation command
                        SendKeys.SendWait("{LEFT}"); // Left arrow
                    });

                    // Wait for file to be modified
                    await WaitForFileSaveAsync(currentFile);

                    progress?.Report((i + 1) * 100 / numOfFiles);
                }

                LogMessage?.Invoke("Rotation completed");
            }
            catch (OperationCanceledException)
            {
                LogMessage?.Invoke("Operation cancelled");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error: {ex.Message}");
            }
        }

        // TODO: add better file change detection
        private async Task WaitForFileSaveAsync(string filePath)
        {
            DateTime startTime = DateTime.Now;
            FileInfo fileInfo = new FileInfo(filePath);
            DateTime lastWriteTime = fileInfo.LastWriteTime;
            long lastFileSize = fileInfo.Length;

            while (true)
            {
                if (_exitLoop || _cts.Token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(500); // Check every 500ms

                fileInfo.Refresh();
                if (fileInfo.LastWriteTime != lastWriteTime || fileInfo.Length != lastFileSize)
                {
                    LogMessage?.Invoke($"File saved: {Path.GetFileName(filePath)}");
                    break;
                }

                if ((DateTime.Now - startTime).TotalSeconds > 20) // Maximum wait time of 20 seconds
                {
                    LogMessage?.Invoke($"Timeout waiting for file save: {Path.GetFileName(filePath)}");
                    break;
                }
            }

            // Additional delay to ensure file is completely saved
            await Task.Delay(1000);
        }

        public void Stop()
        {
            _exitLoop = true;
            _cts?.Cancel();
        }
    }
}