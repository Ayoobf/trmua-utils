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
        private static string REGEX = @"^(?:\d{3}-\d{4}(?:\s*\(\d+\))?|\d{3}(?:-[A-Z])?|\d{3}-\d{4}\.[A-Z]|\d{4})$";

        public async Task AutoMoveAsync(string targetFolder, string destinationFolder, Dispatcher dispatcher)
        {
            _cts= new CancellationTokenSource();
            try 
            {
                await AutoMoveActionAsync(targetFolder, destinationFolder, dispatcher);
            }
            catch (Exception ex)
            {
            
            }
        }

        private async Task AutoMoveActionAsync(string targetFolder, string destinationFolder, Dispatcher dispatcher)
        {
            if (!Directory.Exists(targetFolder))
            {
                LogMessage?.Invoke($"Folder {targetFolder} does not exist");
                _cts?.Cancel();
                return; 
            }

            await dispatcher.InvokeAsync(() =>
            {
                //Rules: date no later thatn 24hours, matches regex, filecount not greater than 50
                var directory = new DirectoryInfo(targetFolder).GetFiles(REGEX);
                LogMessage.Invoke($"found {directory.Length} files in {targetFolder}");

            });
        }

        public void Stop()
        {
            _cts?.Cancel();
        }
    }
}
