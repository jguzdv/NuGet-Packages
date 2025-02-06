using Microsoft.AspNetCore.Components;

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

/// <summary>
/// Observes the busy state of tasks and raises an event when the state changes.
/// </summary>
public class BusyStateObserver
{
    private readonly HashSet<Task> _tasks;

    private BusyState _busyState;

    /// <summary>
    /// Raised when the busy state changes.
    /// </summary>
    public EventCallback<BusyState> BusyStateChanged { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusyStateObserver"/> class.
    /// </summary>
    public BusyStateObserver()
    {
        _tasks = new HashSet<Task>();
    }

    /// <summary>
    /// Adds a task to the observer, it'll be automatically removed upon completion.
    /// </summary>
    public void AddTask(Task task)
    {
        if (task?.IsCompleted == false)
        {
            _tasks.Add(task);
            _ = task?.ContinueWith(t =>
            {
                InvokeStateHasChanged();
            });

            InvokeStateHasChanged();
        }
    }

    private void InvokeStateHasChanged()
    {
        var busyStates = _tasks
            .Select(x => x.GetBusyState())
            .Distinct()
            .ToList();

        _tasks.RemoveWhere(x => x.IsCompleted);

        BusyState nextBusyState = BusyState.Unknown;

        if (busyStates.Any(x => x == BusyState.Busy))
            nextBusyState = BusyState.Busy;
        else if (busyStates.All(x => x == BusyState.Finished))
            nextBusyState = BusyState.Finished;
        else if (busyStates.Any(x => x == BusyState.Failed))
            nextBusyState = BusyState.Failed;

        if(_busyState != nextBusyState)
        {
            _busyState = nextBusyState;
            BusyStateChanged.InvokeAsync(_busyState);
        }
    }
}