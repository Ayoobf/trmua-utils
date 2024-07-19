using System.IO;
using System.Windows.Threading;

namespace trmua_utils.utils
{
    public class RotateFile
    {
        private CancellationTokenSource _cts;
        public event Action<string> LogMessage;

        public async Task RotateFilesAsync(string folderPath, int subfolderToBeLeftOut, IProgress<int> progress, Dispatcher dispatcher)
        {
            _cts = new CancellationTokenSource();
            try
            {
                //LogMessage?.Invoke($"Starting rotation in folder: {folderPath}");
                var files = Directory.GetFiles(folderPath);
                int numOfFiles = files.Length - subfolderToBeLeftOut;
                //LogMessage?.Invoke($"Found {numOfFiles} files to rotate");

                for (int i = 0; i < numOfFiles; i++)
                {
                    if (_cts.Token.IsCancellationRequested)
                    {
                        //LogMessage?.Invoke("Operation cancelled");
                        break;
                    }

                    //LogMessage?.Invoke($"Rotating file {i + 1} of {numOfFiles}");

                    // Simulate key presses on the UI thread
                    await dispatcher.InvokeAsync(() =>
                    {
                        Thread.Sleep(2000);
                        //LogMessage?.Invoke("waiting 2 seconds");
                        SimulateKeyPress("^.");
                        //LogMessage?.Invoke("Pressed: Ctrl + .");
                        Thread.Sleep(500);
                        //LogMessage?.Invoke("waiting 3 seconds");
                        SimulateKeyPress("{LEFT}");
                        //LogMessage?.Invoke("Pressed: Left Arrow");
                        Thread.Sleep(4000);
                    });

                    progress?.Report((i + 1) * 100 / numOfFiles);
                }

                //LogMessage?.Invoke("Rotation completed");
            }
            catch (OperationCanceledException)
            {
                //LogMessage?.Invoke("Operation cancelled");
            }
            catch (Exception ex)
            {
                //LogMessage?.Invoke($"Error: {ex.Message}");
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private void SimulateKeyPress(string keys)
        {
            SendKeys.SendWait(keys);
        }
    }
}