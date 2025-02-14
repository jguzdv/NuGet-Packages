class Modal {
    constructor(
        private _dialogId: string,
        private _dotnetModal: any
    )
    { }

    public openDialog() {
        const dialogElement = document.getElementById(this._dialogId) as HTMLDialogElement;
        dialogElement.showModal();
        dialogElement.addEventListener("close", this.onClose);
    }

    public closeDialog() {
        const dialogElement = document.getElementById(this._dialogId) as HTMLDialogElement;
        dialogElement.removeEventListener("close", this.onClose);

        dialogElement.close();
    }

    private onClose = () => {
        const dialogElement = document.getElementById(this._dialogId) as HTMLDialogElement;
        dialogElement.removeEventListener("close", this.onClose);

        this._dotnetModal.invokeMethodAsync("OnClose");
    }
}

export function createModal(guid: string, dotnetref: any) {
    return new Modal(guid, dotnetref);
};