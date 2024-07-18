using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.UI.Input.Preview.Injection;
using Windows.System;

namespace trmua_utils.utils
{
    public class RotateFile
    {
        private CancellationTokenSource? _cts;
        public event Action<string>? LogMessage;
        private InputInjector _injector;

        public RotateFile()
        {
            _injector = InputInjector.TryCreate();
        }

        public async Task RotateFilesAsync(string folderPath, int subfolderToBeLeftOut, IProgress<int> progress, Dispatcher dispatcher)
        {
            _cts = new CancellationTokenSource();
            try
            {
                LogMessage?.Invoke($"Starting rotation in folder: {folderPath}");
                var files = Directory.GetFiles(folderPath);
                int numOfFiles = files.Length - subfolderToBeLeftOut;
                LogMessage?.Invoke($"Found {numOfFiles} files to rotate");

                for (int i = 0; i < numOfFiles; i++)
                {
                    if (_cts.Token.IsCancellationRequested)
                    {
                        LogMessage?.Invoke("Operation cancelled");
                        break;
                    }

                    LogMessage?.Invoke($"Rotating file {i + 1} of {numOfFiles}");

                    // Simulate key presses on the UI thread
                    await dispatcher.InvokeAsync(() =>
                    {
                        Thread.Sleep(2000);
                        SimulateKeyPress(VirtualKey.Control, VirtualKey.Decimal);
                        LogMessage?.Invoke("Pressed: Ctrl + .");
                        Thread.Sleep(1000);
                        SimulateKeyPress(VirtualKey.Left);
                        LogMessage?.Invoke("Pressed: Left Arrow");
                    });

                    progress?.Report((i + 1) * 100 / numOfFiles);
                    await Task.Delay(4000, _cts.Token);
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
            _cts?.Cancel();
        }

        private void SimulateKeyPress(params VirtualKey[] keys)
        {
            foreach (var key in keys)
            {
                var info = new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)key,
                    KeyOptions = InjectedInputKeyOptions.None
                };

                _injector.InjectKeyboardInput(new[] { info });
            }

            foreach (var key in keys)
            {
                var info = new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)key,
                    KeyOptions = InjectedInputKeyOptions.KeyUp
                };

                _injector.InjectKeyboardInput(new[] { info });
            }
        }
    }
}