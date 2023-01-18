namespace JGUZDV.CQRS.Commands
{
    public class NotFoundResult : ErrorBase
    {
        internal NotFoundResult() : base("NotFound")
        { }
    }
}
