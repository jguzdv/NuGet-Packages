namespace JGUZDV.CQRS.Commands
{
    public class UnauthorizedResult : ErrorBase
    {
        internal UnauthorizedResult() : base("AuthFailed") { }
    }
}
