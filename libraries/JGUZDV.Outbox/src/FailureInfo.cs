namespace JGUZDV.Outbox;

/// <summary>
/// Represents information about a failure that occurred during the processing of an outbox message.
/// </summary>
public class FailureInfo
{
    /// <summary>
    /// Gets or sets the date and time when the failure occurred.
    /// </summary>
    public required DateTimeOffset FailedAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for the failure.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the stack trace associated with the failure.
    /// </summary>
    public string? StackTrace { get; set; }
}
