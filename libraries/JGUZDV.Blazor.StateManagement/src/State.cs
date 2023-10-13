using System.ComponentModel;
using System.Reflection;

namespace JGUZDV.Blazor.Statemanagement;

//TODO: hat die Registrierung über das Interface tatsächlich einen Vorteil für den Anwender?
/// <summary>
/// <see cref="State{T}"/> should wrap your state and be used as a Property in e. g. <see cref="StateListener" /> components, which listen on default to <see cref="IState{T}.StateChanged"/>.
/// It can be used without or with dependency injection preferably registered as <see cref="IState{T}"/> 
/// </summary>
/// <typeparam name="T"></typeparam>
public class State<T> : IState<T>
       where T : INotifyPropertyChanged
{
    private List<INotifyPropertyChanged> _observables;

    private Dictionary<object, List<PropertyData>> _children = new();

    public State(T value)
    {
        _observables = new();
        Value = value;

        RegisterStateChanged(value);
    }

    private void RegisterStateChanged(INotifyPropertyChanged observable)
    {
        observable.PropertyChanged += TriggerStateChanged;
        _observables.Add(observable);

        List<PropertyData> children = new();
        foreach (var propertyInfo in observable.GetType().GetProperties())
        {
            //Wir könnten hier zur Laufzeit erzwingen, dass alle nested Klassen INotifyPropertyChanged erfüllen müssen
            if (propertyInfo.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) != null)
            {
                var propertyValue = (INotifyPropertyChanged?)propertyInfo.GetValue(observable);

                children.Add(new PropertyData(propertyInfo, propertyValue));

                if (propertyValue == null) { continue; }
                RegisterStateChanged(propertyValue);
            }
        }
        _children.Add(observable, children);
    }

    private void UnregisterProperty(INotifyPropertyChanged observable)
    {
        observable.PropertyChanged -= TriggerStateChanged;
        _observables.Remove(observable);

        foreach (var child in _children[observable])
        {
            if (child.Value != null)
                UnregisterProperty(child.Value);
        }
        _children.Remove(observable);
    }

    private void TriggerStateChanged(object? s, PropertyChangedEventArgs e)
    {
        if (s != null)
        {
            var children = _children[s];
            var changedProperty = children.FirstOrDefault(x => x.Info.Name == e.PropertyName);

            if (changedProperty != null)
            {
                //Unregister old property value
                if (changedProperty.Value != null)
                {
                    UnregisterProperty(changedProperty.Value);
                    changedProperty.Value = null;
                }

                //Register new Property value
                var newValue = (INotifyPropertyChanged?)changedProperty.Info.GetValue(s);
                changedProperty.Value = newValue;
                if (newValue != null)
                {
                    RegisterStateChanged(newValue);
                }
            }
        }

        StateChanged?.Invoke(new(s, e.PropertyName));
    }

    public void Dispose()
    {
        foreach (var observable in _observables)
        {
            observable.PropertyChanged -= TriggerStateChanged;
        }
    }

    public T Value { get; private set; }

    public event Action<StateChangedEventArgs>? StateChanged;

    private class PropertyData
    {
        public PropertyData(PropertyInfo info, INotifyPropertyChanged? value)
        {
            Info = info;
            Value = value;
        }

        public PropertyInfo Info { get; set; }
        public INotifyPropertyChanged? Value { get; set; }
    }
}

public interface IState<out T> : IDisposable
    where T : INotifyPropertyChanged
{
    T Value { get; }

    event Action<StateChangedEventArgs>? StateChanged;
};

public class StateChangedEventArgs
{
    public StateChangedEventArgs(object? sender, string? propertyName)
    {
        Sender = sender;
        PropertyName = propertyName;
    }

    public object? Sender { get; }
    public string? PropertyName { get; }
}

