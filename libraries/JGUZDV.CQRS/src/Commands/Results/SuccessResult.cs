namespace JGUZDV.CQRS.Commands
{
    public class SuccessResult : CommandResult
    {
        public SuccessResult()
        {
            IsSuccess = true;
        }
    }
}
