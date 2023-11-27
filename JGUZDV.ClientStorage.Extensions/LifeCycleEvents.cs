namespace JGUZDV.ClientStorage.Extensions;

/// <summary>
/// Simple implementation for ILifeCycleEvents <see cref="Stopped"/> and <see cref="Resumed"/> 
/// must be triggered by the consumer using <see cref="TriggerStopped(object?, EventArgs)"/> 
/// and <see cref="TriggerResumed(object?, EventArgs)"/>
/// </summary>
public class LifeCycleEvents : ILifeCycleEvents
{

    /// <summary>
    /// Trigger the stopped event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void TriggerStopped(object? sender, EventArgs args) => Stopped?.Invoke(sender, args);

    /// <summary>
    /// Trigger the resumed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void TriggerResumed(object? sender, EventArgs args) => Resumed?.Invoke(sender, args);

    /// <inheritdoc />
    public event EventHandler? Stopped;

    /// <inheritdoc />
    public event EventHandler? Resumed;
}
