using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace JGUZDV.Blazor.StateManagement.Components;

public partial class StateView<T> : StateListener
    where T : INotifyPropertyChanged
{
    [Parameter]
    public IState<T>? State { get; set; }

    [Parameter]
    [EditorRequired]
    public RenderFragment<T> ChildContent { get; set; } = default!;

    [Inject]
    public IServiceProvider Services { get; set; } = default!;

    [Parameter]
    public Action<StateChangedEventArgs>? OnPropertyChanged { get; set; }

    protected override void OnInitialized()
    {
        if (State == null)
        {
            State = Services.GetRequiredService<IState<T>>();
        }

        base.OnInitialized();
    }

    public override void PropertyChangedEventHandler(StateChangedEventArgs args)
    {
        if (OnPropertyChanged != null)
        {
            OnPropertyChanged(args);
        }
        else
        {
            base.PropertyChangedEventHandler(args);
        }
    }
}
