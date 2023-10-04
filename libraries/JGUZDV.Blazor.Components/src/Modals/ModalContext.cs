namespace JGUZDV.Blazor.Components.Modals
{
    public abstract class ModalContext
    {
        internal Modal? Parent { get; set; }
        public bool Dismissed { get; set; }

        public Task ShowModal() {
            if (Parent == null)
                throw new InvalidOperationException($"{nameof(Parent)} needs to be set. Assign this object as ModalContext of a Modal to archieve this.");

            return Parent!.ShowModal();
        }

        public void CloseModal() {
            Parent?.CloseModal();
        }
    }

    public class ModalContext<TModel> : ModalContext
    {
        public required TModel Model { get; set; }
    }
}
