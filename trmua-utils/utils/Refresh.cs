using System.Windows.Threading;

namespace trmua_utils.utils
{
    public class Refresh
    {
        private CancellationTokenSource? _cts;
        public event Action<string>? LogMessage;
        private bool _exitLoop = false;

        public async Task RefreshAsync(string TargetApplication, Dispatcher dispatcher)
        {
            _cts = new CancellationTokenSource();
            int delay = 5000;
            _exitLoop = false;
            try
            {

                LogMessage.Invoke($"Refresh started, refreshing window every {delay / 1000}");
                while (true)
                {
                    if (_exitLoop || _cts.IsCancellationRequested)
                    {
                        LogMessage.Invoke($"Operation cancelled");
                        break;
                    }

                    await dispatcher.InvokeAsync(() =>
                    {
                        Thread.Sleep(1000);
                        LogMessage.Invoke($"Refreshed: {DateTime.Now}");
                        SendKeys.SendWait("^r");
                    });
                    await Task.Delay(delay);
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
        }


        public void Stop()
        {
            _cts.Cancel();
            _exitLoop = true;
        }
    }
}
