using Microsoft.AspNetCore.Components.Web;

namespace JGUZDV.Blazor.Components.Extensions
{
    public static class BrowserEventExtensions
    {
        public static async Task OnEnter(this KeyboardEventArgs e, Func<Task> action)
        {
            if (e.Code == "Enter")
            {
                await action();
            }
        }

        public static void OnEnter(this KeyboardEventArgs e, Action action)
        {
            if (e.Code == "Enter")
            {
                action();
            }
        }
    }
}
