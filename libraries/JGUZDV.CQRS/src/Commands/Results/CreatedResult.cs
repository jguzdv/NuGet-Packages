namespace JGUZDV.CQRS.Commands
{
    public class CreatedResult : CommandResult
    {
        internal CreatedResult(object createdId) : base()
        {
            IsSuccess = true;
            CreatedId = createdId;
        }

        public object CreatedId { get; }
        public string? CreatedAtUrl { get; set; }
    }
}
