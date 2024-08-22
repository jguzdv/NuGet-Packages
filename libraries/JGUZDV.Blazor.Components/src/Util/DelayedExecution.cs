namespace JGUZDV.Blazor.Components.Util
{
    public class DelayedExecution
    {
        private readonly TimeSpan _executionDelay;
        private readonly bool _cancelOnExecute;
        private CancellationTokenSource _tokenSource;

        public CancellationToken Token => _tokenSource.Token;

        /// <summary>
        /// Constructor with sensible defaults for keyboard input
        /// </summary>
        /// <param name="cancelOnExecute">Flag if previous task is cancelled.</param>
        /// <param name="millisecondDelay">The delay in milliseconds</param>
        public DelayedExecution(bool cancelOnExecute = true, int millisecondDelay = 333)
            : this(TimeSpan.FromMilliseconds(millisecondDelay))
        {
            _cancelOnExecute = cancelOnExecute;
        }

        public DelayedExecution(int millisecondDelay)
            : this(TimeSpan.FromMilliseconds(millisecondDelay))
        { }

        public DelayedExecution(TimeSpan executionDelay)
        {
            _tokenSource = new CancellationTokenSource();
            _executionDelay = executionDelay;
        }


        public async Task ExecuteDelayed(Action<CancellationToken> action)
        {
            if (_cancelOnExecute)
                Cancel();

            var ct = _tokenSource.Token;

            await Task.Delay(_executionDelay, ct);
            if (ct.IsCancellationRequested)
                return;

            action.Invoke(ct);
        }

        public async Task<TResult> ExecuteDelayed<TResult>(Func<CancellationToken, Task<TResult>> action, Func<TResult>? resultIfCancelled)
        {
            if (_cancelOnExecute)
                Cancel();

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
            if (!_tokenSource.TryReset())
            {
                _tokenSource.Dispose();
                _tokenSource = new();
            }
        }
    }
}
