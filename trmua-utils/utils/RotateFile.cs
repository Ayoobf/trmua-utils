using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace trmua_utils.utils
{
    public class RotateFile
    {
        private CancellationTokenSource? _cts;
        public event Action<string>? LogMessage;
        private bool _exitLoop = false;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);


        public async Task RotateFilesAsync(string targetApplication, string folderPath, int subfolderToBeLeftOut, IProgress<int> progress, Dispatcher dispatcher)
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
                        IntPtr hWnd = WindowFinder.FindWindowByTitle(targetApplication);
                        if (hWnd != IntPtr.Zero)
                        {
                            SetForegroundWindow(hWnd);
                            Thread.Sleep(1000);
                            SendKeys.SendWait("^."); // Ctrl + .
                            Thread.Sleep(1000); // Short delay after rotation command
                            SendKeys.SendWait("{RIGHT}"); // Right arrow
                        }
                        else
                        {
                            LogMessage?.Invoke($"Window '{targetApplication}' not found");
                            LogMessage?.Invoke($"Cancelling Operation, Please open your image before running \"Rotate\"");
                            _cts.Cancel();                        
                        }

                    });

                    // Wait for a fixed amount of time
                    await Task.Delay(7000); // Wait for 7 seconds

                    LogMessage?.Invoke($"Finished processing: {Path.GetFileName(currentFile)}");

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

        public void Stop()
        {
            _exitLoop = true;
            _cts?.Cancel();
        }
    }
}