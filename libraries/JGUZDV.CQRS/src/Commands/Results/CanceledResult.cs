namespace JGUZDV.CQRS.Commands
{
    public class CanceledResult : ErrorBase
    {
        internal CanceledResult() : base("Canceled") { }
    }
}
