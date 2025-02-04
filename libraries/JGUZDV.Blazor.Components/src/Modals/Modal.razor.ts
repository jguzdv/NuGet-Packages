export function OpenDialog(guid: string) {
    var dialog = document.getElementById(guid) as HTMLDialogElement;
    dialog?.showModal();
};

export function CloseDialog(guid: string) {
    var dialog = document.getElementById(guid) as HTMLDialogElement;
    dialog?.close();
};