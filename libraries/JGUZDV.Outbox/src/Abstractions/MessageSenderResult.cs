namespace JGUZDV.Outbox.Abstractions;

public class MessageSenderResult
{
    public bool Success => Exception == null && MessageGenerationException == null;
    public Exception? Exception { get; set; }

    public MessageResultState MessageGenerationState { get; set; }
    public Exception? MessageGenerationException { get; set; }
}