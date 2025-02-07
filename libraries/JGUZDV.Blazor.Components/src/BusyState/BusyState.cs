namespace JGUZDV.Blazor.Components;

/// <summary>
/// Represents the state of a task.
/// </summary>
public enum BusyState
{
    /// <summary>
    /// The state is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// The task is busy. E.g. it is running.
    /// </summary>
    Busy,

    /// <summary>
    /// The task has finished successfully.
    /// </summary>
    Finished,

    /// <summary>
    /// The task has failed.
    /// </summary>
    Failed
};
