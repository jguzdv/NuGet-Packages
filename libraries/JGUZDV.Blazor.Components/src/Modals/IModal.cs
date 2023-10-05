using Microsoft.AspNetCore.Components;

namespace JGUZDV.Blazor.Components.Modals
{
    public interface IModal
    {
        RenderFragment ChildContent { get; }

        void CloseModal(ModalResult result);
    }
}
