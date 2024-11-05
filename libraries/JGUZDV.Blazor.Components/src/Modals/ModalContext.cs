using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.Blazor.Components.Modals
{
    /// <summary>
    /// Contains the Modal itself and the model that the modal is working with.
    /// </summary>
    public class ModalContext<TModel>
        where TModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModalContext{TModel}"/> class.
        /// </summary>
        /// <param name="modal">The modal instance.</param>
        /// <param name="model">The model instance.</param>
        [SetsRequiredMembers]
        public ModalContext(Modal<TModel> modal, TModel model)
        {
            Modal = modal;
            Model = model;
        }

        /// <summary>
        /// The modal instance.
        /// </summary>
        public Modal<TModel> Modal { get; }

        /// <summary>
        /// The model instance.
        /// </summary>
        public TModel Model { get; set; }
    }
}
