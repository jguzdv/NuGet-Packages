using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Reflection;

namespace JGUZDV.Blazor.Statemanagement;

/// <summary>
/// Components with Properties of type <see cref="IState{T}"/> can inherit from StateListener to automatically register 
/// (the overrideable) <see cref="PropertyChangedEventHandler(StateChangedEventArgs)"/> to <see cref="IState{T}.StateChanged"/>
/// </summary>
public class StateListener : ComponentBase, IDisposable
{
    private List<IState<INotifyPropertyChanged>> _states = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        foreach (var property in GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (property.PropertyType.IsGenericType
                //allow IState and State for now
                && (property.PropertyType.GetGenericTypeDefinition() == typeof(IState<>) || property.PropertyType.GetGenericTypeDefinition() == typeof(State<>)))
            {
                object? val = property.GetValue(this);
                var state = val as IState<INotifyPropertyChanged>;
                if (state == null) { continue; }

                state.StateChanged += PropertyChangedEventHandler;
                _states.Add(state);
            }
        }
    }

    public virtual void PropertyChangedEventHandler(StateChangedEventArgs args)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        foreach (var state in _states)
        {
            state.StateChanged -= PropertyChangedEventHandler;
        }
    }
}
