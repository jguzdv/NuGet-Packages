export class closeListener {
    constructor() {
        this.handleCloseEvent = this.handleCloseEvent.bind(this);
    }
    ;
    addListener(guid, reference) {
        var dialog = document.getElementById(guid);
        dialog.addEventListener("close", this.handleCloseEvent(reference));
    }
    removeListener(guid, reference) {
        var dialog = document.getElementById(guid);
        dialog.removeEventListener("close", this.handleCloseEvent(reference));
    }
    handleCloseEvent(reference) {
        return () => reference.invokeMethodAsync("Close");
    }
}
const listener = new closeListener();
export function OpenDialog(guid) {
    var dialog = document.getElementById(guid);
    dialog.showModal();
}
;
export function CloseDialog(guid, reference) {
    var dialog = document.getElementById(guid);
    listener.removeListener(guid, reference);
    dialog.close();
}
;
export function CloseListener(guid, reference) {
    listener.addListener(guid, reference);
}
;
//# sourceMappingURL=Modal.js.map