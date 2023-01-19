namespace JGUZDV.CQRS.Queries
{
    public class CanceledResult<T> : ErrorBase<T>
    {
        public CancellationToken CancellationToken { get; }

        internal CanceledResult(CancellationToken cancellationToken) : base("Canceled") {
            CancellationToken = cancellationToken;
        }
    }
}
