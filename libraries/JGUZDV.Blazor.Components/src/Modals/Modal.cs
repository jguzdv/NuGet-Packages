using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Components;

namespace JGUZDV.Blazor.Components.Modals
{
    public interface IModal
    {
        ModalContext ModalContext { get; }
        RenderFragment ChildContent { get; }

        Task ShowModal();
        void CloseModal();
    }

    public class Modal<TModel> : ComponentBase, IModal
    {
        [Inject, NotNull]
        internal ModalService? ModalService { get; set; }

        [Parameter, NotNull, EditorRequired]
        public ModalContext<TModel>? ModalContext { get; set; }

        [Parameter]
        public RenderFragment<ModalContext<TModel>>? ChildContent { get; set; }


        [Parameter]
        public bool ResponsiveSizes { get; set; } = true;

        [Parameter]
        public bool FullscreenMode { get; set; } = false;


        ModalContext IModal.ModalContext => ModalContext;
        RenderFragment IModal.ChildContent => ChildContent?.Invoke(ModalContext);

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if(ModalContext.Parent != null && ModalContext.Parent != this)
            {
                throw new InvalidOperationException("ModalContext should not be reused.");
            }

            ModalContext.Parent = this;
        }


        public Task ShowModal() => ModalService.ShowModal(this);

        public void CloseModal() => ModalService.CloseModal(this);


        public void Dismiss()
        {
            ModalContext.Dismissed = true;
            ModalService.CloseModal(this);
        }
    }
}
