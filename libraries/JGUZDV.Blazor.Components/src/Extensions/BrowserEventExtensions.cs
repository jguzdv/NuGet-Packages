using Microsoft.AspNetCore.Components.Web;

namespace JGUZDV.Blazor.Components.Extensions
{
    /// <summary>
    /// Brower event extensions
    /// </summary>
    public static class BrowserEventExtensions
    {
        /// <summary>
        /// Executes the specified function on enter
        /// </summary>
        /// <returns></returns>
        public static async Task OnEnter(this KeyboardEventArgs e, Func<Task> action)
        {
            if (e.Code == "Enter")
            {
                await action();
            }
        }

        /// <summary>
        /// Executes the specified action on enter
        /// </summary>
        /// <param name="e"></param>
        /// <param name="action"></param>
        public static void OnEnter(this KeyboardEventArgs e, Action action)
        {
            if (e.Code == "Enter")
            {
                action();
            }
        }
    }
}
