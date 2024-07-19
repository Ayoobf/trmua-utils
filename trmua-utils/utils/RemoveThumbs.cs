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
            try
            {
                await dispatcher.InvokeAsync(() =>
                {
                    var mapsInDir = Directory.GetFiles(folderName);
                    Regex? re = new Regex(strRegex);
                    foreach (var map in mapsInDir)
                    {
                        if (re.IsMatch(map))
                        {
                            File.Delete(map);
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error: {ex.Message}");
            }


        }
        public void Stop() => _cts.Cancel();


    }
}
