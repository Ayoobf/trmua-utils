using System.Windows.Threading;

namespace trmua_utils.utils
{
    public class Refresh
    {
        private CancellationTokenSource? _cts;
        public event Action<string>? LogMessage;
        private const int DelayMs = 5000;

        public async Task RefreshAsync(string TargetApplication, Dispatcher dispatcher)
        {
            _cts = new CancellationTokenSource();
            int delay = 5000;
            try
            {
                LogMessage?.Invoke($"Refresh started, refreshing window every {DelayMs / 1000} seconds");
                while (!_cts.Token.IsCancellationRequested)
                {
                    await RefreshActionAsync(dispatcher);
                    await Task.Delay(DelayMs, _cts.Token);
                }

            }
            catch (OperationCanceledException)
            {
                LogMessage.Invoke($"Operation cancelled");
            }
            catch (Exception ex)
            {
                LogMessage.Invoke($"{ex}");
            }
            finally
            {
                LogMessage?.Invoke("Refresh operation stopped");
            }
        }
        private async Task RefreshActionAsync(Dispatcher dispatcher)
        {
            await dispatcher.InvokeAsync(() =>
            {
                Thread.Sleep(1000);
                LogMessage?.Invoke($"Refreshed: {DateTime.Now}");
                SendKeys.SendWait("^r");
            });
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
