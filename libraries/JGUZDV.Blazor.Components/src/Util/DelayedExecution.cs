namespace JGUZDV.Blazor.Components.Util
{
    internal class DelayedExecution
    {
        private readonly TimeSpan _executionDelay;
        private CancellationTokenSource _tokenSource;

        public CancellationToken Token => _tokenSource.Token;


        public DelayedExecution(int millisecondDelay) 
            :this(TimeSpan.FromMilliseconds(millisecondDelay))
        { }

        public DelayedExecution(TimeSpan executionDelay)
        {
            _tokenSource = new CancellationTokenSource();
            _executionDelay = executionDelay;
        }


        public async Task ExecuteDelayed(Action<CancellationToken> action)
        {
            var ct = _tokenSource.Token;

            await Task.Delay(_executionDelay, ct);
            if (ct.IsCancellationRequested)
                return;

            action.Invoke(ct);
        }
        
        public async Task<TResult> ExecuteDelayed<TResult>(Func<CancellationToken, Task<TResult>> action, Func<TResult>? resultIfCancelled)
        {
            var ct = _tokenSource.Token;

            await Task.Delay(_executionDelay, ct);
            if (ct.IsCancellationRequested)
            {
                if (resultIfCancelled is null)
                    ct.ThrowIfCancellationRequested();

                return resultIfCancelled!();
            }

            return await action(ct);
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
            if (!_tokenSource.TryReset()) {
                _tokenSource.Dispose();
                _tokenSource = new();
            }
        }
    }
}
