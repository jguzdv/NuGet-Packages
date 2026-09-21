namespace JGUZDV.Outbox.Abstractions;

public class MessageFactoryResult<T>
{
    public T? Message { get; set; }
    public ResultState State { get; set; }

    public Exception? Exception { get; set; }

    public enum ResultState
    {
        Error,
        TransientError,
        PermanentError,
        Success,
        NotSupported
    }
}


