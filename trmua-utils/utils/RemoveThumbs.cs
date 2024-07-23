using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace trmua_utils.utils
{
    internal class RemoveThumbs
    {
        private CancellationTokenSource? _cts;
        public event Action<string>? LogMessage;
        private static string strRegex = @".*\.thumb";

        public async Task RemoveThumbsAsync(string folderName, IProgress<int> progress, Dispatcher dispatcher)
        {
            _cts = new CancellationTokenSource();
            List<string> matchingFiles;
            try
            {
                await dispatcher.InvokeAsync(() =>
                {
                    var maps = Directory.GetFiles(folderName);
                    Regex? re = new Regex(strRegex);

                    matchingFiles = maps.Where(map => re.IsMatch(map)).ToList();

                    if (matchingFiles.Count == 0)
                    {
                        LogMessage?.Invoke($"No maps found");
                        return;
                    }

                    LogMessage?.Invoke($"Found {matchingFiles.Count} files matching the criteria");

                    foreach (var map in matchingFiles)
                    {
                        if (re.IsMatch(map))
                        {
                            File.Delete(map);
                            LogMessage?.Invoke($"deleted {map}");
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error: {ex.Message}");
            }


        }
        public void Stop() => _cts?.Cancel();


    }
}
