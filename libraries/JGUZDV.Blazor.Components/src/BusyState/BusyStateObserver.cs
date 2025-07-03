using Microsoft.AspNetCore.Components;

namespace JGUZDV.Blazor.Components;

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
    /// Returns the current busy state of the observer.
    /// </summary>
    public BusyState CurrentState => _busyState;

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