namespace trmua_utils.utils
{
    internal class RemoveThumbs
    {
        private CancellationTokenSource _cts;
        public event Action<string> LogMessage;

        public async Task RemoveThumbsAsync()
        {
            _cts = new CancellationTokenSource();
            try
            {
            }
            catch (Exception ex)
            {

            }


        }

    }
}
