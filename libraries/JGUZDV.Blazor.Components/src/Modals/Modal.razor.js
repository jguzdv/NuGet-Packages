export function OpenDialog(guid) {
    var dialog = document.getElementById(guid);
    dialog === null || dialog === void 0 ? void 0 : dialog.showModal();
}
;
export function CloseDialog(guid) {
    var dialog = document.getElementById(guid);
    dialog === null || dialog === void 0 ? void 0 : dialog.close();
}
;
