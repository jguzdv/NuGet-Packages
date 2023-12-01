export function OpenDialog(guid) {
    var dialog = document.getElementById(guid);
    dialog.showModal();
};

export function CloseDialog(guid) {
    var dialog = document.getElementById(guid);
    dialog.close();
};