class Modal {
    _dialogId;
    _dotnetModal;
    constructor(_dialogId, _dotnetModal) {
        this._dialogId = _dialogId;
        this._dotnetModal = _dotnetModal;
    }
    openDialog() {
        const dialogElement = document.getElementById(this._dialogId);
        dialogElement.showModal();
        dialogElement.addEventListener("close", this.onClose);
    }
    closeDialog() {
        const dialogElement = document.getElementById(this._dialogId);
        dialogElement.removeEventListener("close", this.onClose);
        dialogElement.close();
    }
    onClose = () => {
        const dialogElement = document.getElementById(this._dialogId);
        dialogElement.removeEventListener("close", this.onClose);
        this._dotnetModal.invokeMethodAsync("OnClose");
    };
}
export function createModal(guid, dotnetref) {
    return new Modal(guid, dotnetref);
}
;
