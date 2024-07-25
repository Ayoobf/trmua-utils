using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace trmua_utils.utils
{
    internal class RemoveThumbs
    {
        private CancellationTokenSource? _cts;
        public event Action<string>? LogMessage;
        private static string strRegex = @".*\.thumb";
        private Timer? _timer;
        private string? _folderName;
        private Dispatcher? _dispatcher;

        public void StartRemovingThumbs(string folderName, Dispatcher dispatcher)
        {
            _folderName = folderName;
            _dispatcher = dispatcher;
            _cts = new CancellationTokenSource();

            // Perform initial removal
            RemoveThumbsN();

            // Set up timer for constant checking
            _timer = new Timer(5000); // 5000 milliseconds = 5 seconds
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            LogMessage?.Invoke("Thumb removal started. Checking every 5 seconds.");
        }

        public void Stop()
        {
            _cts?.Cancel();
            _timer?.Stop();
            _timer?.Dispose();
            LogMessage?.Invoke("Thumb removal stopped.");
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_cts.IsCancellationRequested)
            {
                Stop();
                return;
            }

            _dispatcher.Invoke(RemoveThumbsN);
        }

        private void RemoveThumbsN()
        {
            try
            {
                var maps = Directory.GetFiles(_folderName);
                Regex? re = new Regex(strRegex);

                var matchingFiles = maps.Where(map => re.IsMatch(map)).ToList();

                if (matchingFiles.Count == 0)
                {
                    LogMessage?.Invoke($"No thumb files found in {_folderName}");
                    return;
                }

                LogMessage?.Invoke($"Found {matchingFiles.Count} thumb files");

                foreach (var map in matchingFiles)
                {
                    File.Delete(map);
                    LogMessage?.Invoke($"Deleted {map}");
                }

                LogMessage?.Invoke($"Finished checking and removing thumb files in {_folderName}");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error: {ex.Message}");
            }
        }
    }
}