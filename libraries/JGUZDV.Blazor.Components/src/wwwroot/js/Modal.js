export function OpenDialog(guid) {
    var dialog = document.getElementById(guid);
    dialog.showModal();
};

export function CloseDialog(guid, reference) {
    var dialog = document.getElementById(guid);
    dialog.removeEventListener("close", () => { reference.invokeMethodAsync("Close"); });
    dialog.close();
};

export function CloseListener(guid, reference) {
    var dialog = document.getElementById(guid);
    dialog.addEventListener("close", () => { reference.invokeMethodAsync("Close"); });
    }