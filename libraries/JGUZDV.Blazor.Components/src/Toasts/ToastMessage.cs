namespace JGUZDV.Blazor.Components.Toasts
{
    /// <summary>
    /// Defines a toast message that can be displayed to the user.
    /// </summary>
    public class ToastMessage
    {
        /// <summary>
        /// Creates a new ToastMessage with the specified level, message, and title.
        /// </summary>
        public ToastMessage(ToastLevel toastLevel, string message, string? title, TimeSpan? autoDismissAfter = null)
        {
            ToastLevel = toastLevel;
            Message = message;
            Title = title;
            AutoDismissAfter = autoDismissAfter;
        }

        /// <summary>
        /// The serverity level of the toast message.
        /// </summary>
        public ToastLevel ToastLevel { get; }

        /// <summary>
        /// The message content of the toast.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The title of the toast message, if any.
        /// </summary>
        public string? Title { get; }

        /// <summary>
        /// The duration after which the toast will be automatically dismissed.
        /// When set, the countdown pauses while the user hovers or focuses the toast (WCAG 2.2.1).
        /// </summary>
        public TimeSpan? AutoDismissAfter { get; }
    }
}
