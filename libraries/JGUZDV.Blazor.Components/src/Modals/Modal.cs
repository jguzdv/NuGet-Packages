using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Components;

namespace JGUZDV.Blazor.Components.Modals
{
    public class Modal : ComponentBase
    {
        [Inject, NotNull]
        internal ModalService? ModalService { get; set; }

        [Parameter, NotNull, EditorRequired]
        public ModalContext? ModalContext { get; set; }

        [Parameter]
        public RenderFragment<ModalContext>? ChildContent { get; set; }


        [Parameter]
        public bool ResponsiveSizes { get; set; } = true;

        [Parameter]
        public bool FullscreenMode { get; set; } = false;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if(ModalContext.Parent != null && ModalContext.Parent != this)
            {
                throw new InvalidOperationException("ModalContext should not be reused.");
            }

            ModalContext.Parent = this;
        }


        internal Task ShowModal() => ModalService.ShowModal(this);

        internal void CloseModal() => ModalService.CloseModal(this);

        internal void Dismiss()
        {
            ModalService.CloseModal(this);
            ModalContext.Dismissed = true;
        }
    }
}
