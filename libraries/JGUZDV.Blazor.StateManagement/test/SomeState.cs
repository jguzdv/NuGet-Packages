using CommunityToolkit.Mvvm.ComponentModel;

namespace JGUZDV.Blazor.StateManagement.Tests
{
    internal partial class SomeState : ObservableObject
    {
        [ObservableProperty]
        private int _propertyOne;

        [ObservableProperty]
        private int _propertyTwo;

        [ObservableProperty]
        private int _propertyThree;
    }
}
