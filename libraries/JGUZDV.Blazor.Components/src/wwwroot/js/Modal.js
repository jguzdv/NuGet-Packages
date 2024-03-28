export function OpenDialog(guid) {
    var dialog = document.getElementById(guid);
    dialog.showModal();
};

export function CloseDialog(guid) {
    var dialog = document.getElementById(guid);
    dialog.close();
};

export function EscapeListener(guid, reference) {
    var dialog = document.getElementById(guid);
    dialog.addEventListener("close", () => {
        reference.invokeMethodAsync("Close");
    })
}


