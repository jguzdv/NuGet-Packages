# Blazor State Management

Sharing state across an app can be cumbersome and error prone. How can we notify other components on state changes? What problems can arise and how can they be solved?
In the following we take a look at some problems and requirements of state management and how the ZDVMainz.BlazorState package can help you to easily share state across components or an entire app. 

## Why you might be interested

1. Easily share state across an app 
2. Independent of the render cycle
3. Dependent components rerender if needed
4. Being able to bind at input fields (MVVM rather than MVU)

## Examples to reveal the issues

First let's first consider the following component with a date input.

```csharp
<input type="date" @bind="Date" />

@code {
    [Parameter]
    public string Date { get; set; }
}
```
The default way of handling changes to ```Date``` would be to add an ```EventCallback<string>```, which is invoked on change, leading to something like:
```csharp
<input type="date" @bind="Date" @bind:after="() => DateChanged.InvokeAsync(Date)" />

@code {
    [Parameter]
    public string Date { get; set; }

    [Parameter]
    public EventCallback<string> DateChanged { get; set; }
}
```
For simple components, there is no problem with this, since it does notify the parent component and is easy to implement. The problem arises when state is complex or shared across a hierachy of multiple components,
since in each layer the EventCallback must be implemented to pass that information through to the root if needed. 

Therefore, let's look at a more elaborate example. Consider we have one component, where 
you can configure multiple values and another component, which depends on some of this data. A straight forward approach could look like this:
```csharp
//Configuring component
<input type="text" @bind="Options.Name" @bind:after="() => NameChanged.InvokeAsync(Options.Name)" />
<input type="text" @bind="Options.Description" @bind:after="() => DescriptionChanged.InvokeAsync(Options.Description)"/>
<input type="check" @bind="Options.IsUrgent" @bind:after="() => IsUrgentChanged.InvokeAsync(Options.IsUrgent)"/>
@code {
    [Parameter]
    public Options Options { get; set; }

    [Parameter]
    EventCallback<string> NameChanged { get; set; }

    [Parameter]
    EventCallback<string> DescriptionChanged { get; set; }

    [Parameter]
    EventCallback<bool> IsUrgentChanged { get; set; }
}
```
and
```csharp
//parent component

<ConfiguringComponent 
    Options="Options" 
    NameChanged="OnNameChanged"
    DescriptionChanged="OnDescriptionChanged"
    IsUrgentChanged="StateHasChanged" />

//markup to render something dependent on the configured options

@code {
    [Parameter]
    public Options Options { get; set; }

    public async Task OnNameChanged(){
        // do stuff
    }

    public async Task OnDescriptionChanged(){
        // do stuff
    }
}
```
Now we can start to see how tedious it is to inform the parent for each possible change. Imagine the common ancestor of both components is several layers above them. Each component in between needs these callbacks!
And if there is no common ancestor, we would further need custom code to share this state.

## Solution

We can let ```Options``` implement ```INotifyPropertyChanged```, but doing this ourselves also involves a lot of boilerplate code in the ```Options``` class as well as in the dependent component to proper
register and deregister the needed events. This even gets worse, since ```Options``` itself has an observable object as a property, which also needs to be handled correctly. 
So let's take a look at how to bypass these problems. 
For implementing the ```INotifyPropertyChanged``` interface we can use the CommunityToolkit.Mvvm like so:
```csharp
//[INotifyPropertyChanged] - this attribute can be used as an alternative to inheritance
public class Options : ObservableObject {
    
    [ObservableProperty]
    string name;

    [ObservableProperty]
    string description;

    [ObservableProperty]
    bool isUrgent;

    //Properties that implement INotifyPropertyChanged are recursively registered 
    //by the state management!
    [ObservableProperty]
    INotifyPropertyChanged someObservableObject;
}
```
As you can see, this package allows us to implement the ```INotifyPropertyChange``` interface with minimal effort and still be explicit which properties should invoke ```OnPropertyChanged```.
This alone simplifies the previous example.

### Updated Example
 As the updated example below shows, consuming components like 
the "Configuring component" can bind directly to the options Properties. But components, which need to be informed of changes need to (de-)register the property changed event manually. 
And what about nested options?

```csharp
//Configuring component
<input type="text" @bind="Options.Name" />
<input type="text" @bind="Options.Description" />
<input type="check" @bind="Options.IsUrgent" />
@code {
    [Parameter]
    public Options Options { get; set; }
}
```
and
```csharp
//parent component

<ConfiguringComponent 
    Options="Options" />

//markup to render something dependent on the configured options

@code {
    [Parameter]
    public Options Options { get; set; }

    override void OnInitialized(){
        Options.PropertyChanged += HandlePropertyChanged;
    }

    public void Dispose(){
        Options.PropertyChanged -= HandlePropertyChanged;
    }

    public void HandlePropertyChanged(object sender, PropertyChangedEventArgs){
        //do stuff - e.g. invoke StateHasChanged
        InvokeAsync(StateHasChanged); 
    }
}
```

### Final Solution
Finally, to further improve our example, we need the ZDVMainz.BlazorState package. First we need to register the options as state in the dependency injection container (it can also be used without DI):
```csharp
//the state class does the magic for registering the
//whole tree of observable objects to the StateChanged event
services.AddScoped<IState<Options>>(_ => new State<Options>(new Options()));
```
and then updating our example to:
```csharp
//Configuring component
<input type="text" @bind="Options.Value.Name"  />
<input type="text" @bind="Options.Value.Description" />
<input type="check" @bind="Options.Value.IsUrgent" />

//this works also! Properties that implement INotifyPropertyChanged are recursively registered
<input type="check" @bind="Options.Value.SomeObservableObject.SomeBoolean" />
@code {
    [Inject] // we could still pass this by Parameter
    public IState<Options> Options { get; set; }

}
```
```csharp
//parent component

//the StateListener base class automatically (de-)register the PropertyChangedEventHandler to the IState<>.StateChanged event of all IState<> properties of this component.
@inherits StateListener

<ConfiguringComponent />

//markup to render something dependent on the configured options


@code {
    [Inject]
    public IState<Options> Options { get; set; }

    //this is optional! - here we can override the handler if necessary
    public override void PropertyChangedEventHandler(StateChangedEventArgs args)
    {
        //the base implementation invokes StateHasChanged
        base.PropertyChangedEventHandler(args);
    }
}
```
With this the dependent component automatically rerenders, when an options value change. 
The ```StateListener``` base class automatically (de-)register ```PropertyChangedEventHandler``` to the ```IState<>.StateChanged``` event of each ```IState<>``` property. The handler 
is overrideable if we want more control and e. g. only rerender if a certain property changes or to execute additional logic on change.

### Alternative Convenience Component
The ```StateView``` component can be used to make only parts of a component listening to state changed (in contrast to the ```StateListener```). 
So here is an updated version of the parent compont, where only the dependent markup listenes on changes!
//parent component

//the StateListener base class automatically (de-)register the PropertyChangedEventHandler to the IState<>.StateChanged event of all IState<> properties of this component.
```
<ConfiguringComponent />

// the state can also be passed as a Parameter
// and the PropertyChangedEventHandler can be passed too
<StateView T="Options"> 
    //markup to render something dependent on the configured options
</StateView>

@code {
    [Inject]
    public IState<Options> Options { get; set; }        
}
```


## Conclusion
So with minimal code, we can share state across components, notify dependent components and even let them rerender without the need to manually (de-)register and manage the events. 
Nonetheless, we have full control to react only on specific changes or trigger additional logic.