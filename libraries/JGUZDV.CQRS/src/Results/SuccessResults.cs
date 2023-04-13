namespace JGUZDV.CQRS
{
    public class SuccessResult : HandlerResult
    {
        internal static SuccessResult GenericSuccess { get; } = new SuccessResult();

        public SuccessResult()
        {
            IsSuccess = true;
        }
    }
}
