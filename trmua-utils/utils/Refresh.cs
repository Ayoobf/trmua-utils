using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace trmua_utils.utils
{
    public class Refresh
    {
        private CancellationTokenSource? _cts;
        public event Action<string>? LogMessage;
        private const int DelayMs = 5000;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public async Task RefreshAsync(string targetApplication, Dispatcher dispatcher)
        {
            _cts = new CancellationTokenSource();
            try
            {
                LogMessage?.Invoke($"Refresh started, refreshing window every {DelayMs / 1000} seconds");
                while (!_cts.Token.IsCancellationRequested)
                {
                    await RefreshActionAsync(dispatcher, targetApplication);
                    await Task.Delay(DelayMs, _cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage?.Invoke("Operation cancelled");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"An error occurred: {ex.Message}");
            }
            finally
            {
                LogMessage?.Invoke("Refresh operation stopped");
            }
        }

        private async Task RefreshActionAsync(Dispatcher dispatcher, string targetApplication)
        {
            await dispatcher.InvokeAsync(() =>
            {
                IntPtr hWnd = WindowFinder.FindWindowByTitle(targetApplication);
                if (hWnd != IntPtr.Zero)
                {
                    SetForegroundWindow(hWnd);
                    Thread.Sleep(100); // Give a moment for the window to come to the foreground
                    SendKeys.SendWait("^r");
                    LogMessage?.Invoke($"Refreshed: {DateTime.Now}");
                }
                else
                {
                    LogMessage?.Invoke($"Window '{targetApplication}' not found");
                }
            });
        }

        public void Stop()
        {
            _cts?.Cancel();
        }
    }
}