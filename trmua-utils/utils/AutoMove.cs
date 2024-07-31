using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace trmua_utils.utils
{
    public class AutoMove
    {
        private CancellationToken? _cts;
        public event Action<string>? LogMessage;

        public async Task AutoMoveAsync(string TargetFolder, string DestinationFolder, Dispatcher dispatcher)
        {
            _cts= new CancellationToken();

        }
    }
}
