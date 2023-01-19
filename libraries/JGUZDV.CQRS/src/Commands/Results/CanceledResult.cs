namespace JGUZDV.CQRS.Commands
{
    public class CanceledResult : ErrorBase
    {
        public CancellationToken CancellationToken { get; }

        internal CanceledResult(CancellationToken cancellationToken) : base("Canceled") {
            CancellationToken = cancellationToken;
        }
    }
}
