using Microsoft.AspNetCore.Components.Web;

namespace JGUZDV.Blazor.Components.Extensions
{
    /// <summary>
    /// Provides extensions for handling browser events, specifically keyboard events with specific keys like "Enter".
    /// </summary>
    public static class BrowserEventExtensions
    {
        /// <summary>
        /// Provides a shortcut to listen for the "Enter" key in a keyboard event and execute an asynchronous action.
        /// </summary>
        public static async Task OnEnter(this KeyboardEventArgs e, Func<Task> action)
        {
            if (e.Code == "Enter")
            {
                await action();
            }
        }


        /// <summary>
        /// Provides a shortcut to listen for the "Enter" key in a keyboard event and execute a synchronous action.
        /// </summary>
        public static void OnEnter(this KeyboardEventArgs e, Action action)
        {
            if (e.Code == "Enter")
            {
                action();
            }
        }
    }
}
