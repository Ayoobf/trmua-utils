using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.DirectoryServices;

namespace trmua_utils.utils
{
    public class AutoMove
    {
        private CancellationTokenSource? _cts;
        public event Action<string>? LogMessage;
        private static string pattern = @"^(?:\d{3}-\d{4}(?:\s*\(\d+\))?|\d{3}(?:-[A-Z])?|\d{3}-\d{4}\.[A-Z]|\d{4}).tiff$";

        public async Task AutoMoveAsync(string targetFolder, string destinationFolder, Dispatcher dispatcher)
        {
            _cts= new CancellationTokenSource();
            try 
            {
                await AutoMoveActionAsync(targetFolder, destinationFolder, dispatcher);
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"{ex}");
            }
        }

        private async Task AutoMoveActionAsync(string targetFolder, string destinationFolder, Dispatcher dispatcher)
        {
            if (!Directory.Exists(targetFolder) || !Directory.Exists(destinationFolder))
            {
                LogMessage?.Invoke($"Folder {targetFolder} does not exist.\n Cancelling operation");
                _cts?.Cancel();
                return; 
            }

            await dispatcher.InvokeAsync(() =>
            {
                try
                {
                    //Rules: date no later thatn 24hours, matches regex, filecount not greater than 50
                    var directory = new DirectoryInfo(targetFolder);
                    Regex re = new Regex(pattern);

                    var matchingFiles = directory.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                        .Where(file => re.IsMatch(file.Name))
                        .OrderByDescending(file => file.LastWriteTime)
                        .Take(100)
                        .ToList();

                    if (matchingFiles.Count is 0)
                    {
                        LogMessage?.Invoke($"found no files in {directory}. Cancelling operation");
                        _cts?.Cancel();
                        return;
                    }


                    LogMessage?.Invoke($"found {matchingFiles.Count} files in {directory}");
                    foreach (var file in matchingFiles)
                    {
                        file.MoveTo(destinationFolder);
                        LogMessage?.Invoke($"moved {file.FullName} to {destinationFolder} ");
                    }
                }
                catch (OperationCanceledException)
                {
                    LogMessage?.Invoke($"operation stoppped");
                }
                catch (Exception e)
                {
                    LogMessage?.Invoke($"ERROR:{e}");

                }
                finally
                {
                    LogMessage?.Invoke($"Done moving. Stopping Operation");
                }

            });
        }

        public void Stop()
        {
            _cts?.Cancel();
        }
    }
}
