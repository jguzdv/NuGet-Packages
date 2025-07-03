namespace JGUZDV.Blazor.Components.Util
{
    /// <summary>
    /// This povides a way to delay the execution of an action or function by a specified time span.
    /// Essentially a debounce mechanism that can be used to prevent rapid execution of actions, such as during keyboard input.
    /// </summary>
    public class DelayedExecution
    {
        private readonly TimeSpan _executionDelay;
        private readonly bool _cancelOnExecute;
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> associated with the current operation.
        /// </summary>
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

        /// <summary>
        /// Constructor that allows specifying a delay in milliseconds.
        /// </summary>
        /// <param name="millisecondDelay">The delay in ms</param>
        public DelayedExecution(int millisecondDelay)
            : this(TimeSpan.FromMilliseconds(millisecondDelay))
        { }

        /// <summary>
        /// Constructor that allows specifying a custom delay.
        /// </summary>
        /// <param name="executionDelay">The delay as Timespan</param>
        public DelayedExecution(TimeSpan executionDelay)
        {
            _tokenSource = new CancellationTokenSource();
            _executionDelay = executionDelay;
        }


        /// <summary>
        /// Executes the provided action after the specified delay.
        /// </summary>
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

        /// <summary>
        /// Executes the provided action after the specified delay and returns a result.
        /// </summary>
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

        /// <summary>
        /// Allows to cancel the current operation and reset the token source.
        /// </summary>
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
