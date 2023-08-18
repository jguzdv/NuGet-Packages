namespace JGUZDV.ClientStorage;

/// <summary>
/// Life cycle events
/// </summary>
public interface ILifeCycleEvents
{
    /// <summary>
    /// Event that is fired if the application is stopped
    /// </summary>
    public event EventHandler? Stopped;

    /// <summary>
    /// Event that is fired if the application is resumed
    /// </summary>
    public event EventHandler? Resumed;
}
