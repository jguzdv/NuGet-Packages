namespace JGUZDV.Blazor.Components;

/// <summary>
/// Provides extension methods for the <see cref="Task"/> class.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Gets the busy state of the task.
    /// </summary>
    public static BusyState GetBusyState(this Task? task)
    {
        if (task == null)
            return BusyState.Unknown;

        if (task.IsCompletedSuccessfully)
            return BusyState.Finished;

        if (task.IsCompleted)
            return BusyState.Failed;

        return BusyState.Busy;
    }
}
