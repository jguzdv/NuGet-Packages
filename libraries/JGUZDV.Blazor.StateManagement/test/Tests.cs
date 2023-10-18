using Bunit;
using Microsoft.Extensions.DependencyInjection;
using JGUZDV.Blazor.StateManagement.Components;

namespace JGUZDV.Blazor.StateManagement.Tests
{
    public class Tests : TestContext
    {
        [Fact]
        public void Test1()
        {
            Services.AddScoped<IState<SomeState>>(x => new State<SomeState>(new()));
            Services.AddScoped<IServiceProvider>(x => Services);

            var stateview = RenderComponent<StateView<SomeState>>(x => x.Add(
                p => p.ChildContent,
                x => $"<p>{x.PropertyOne}</p>"));

            Assert.Equal("0", stateview.Find("p").InnerHtml);

            var state = Services.GetRequiredService<IState<SomeState>>();
            state.Value.PropertyOne = 1;

            Assert.Equal("1", stateview.Find("p").InnerHtml);
        }
    }
}