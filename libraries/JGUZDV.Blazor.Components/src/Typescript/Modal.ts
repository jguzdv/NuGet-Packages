export class closeListener {
    constructor() {
        this.handleCloseEvent = this.handleCloseEvent.bind(this);
    };

    public addListener(guid, reference) {
        var dialog = document.getElementById(guid);
            dialog.addEventListener("close", this.handleCloseEvent(reference));
    }

    public removeListener(guid, reference) {
        var dialog = document.getElementById(guid);
            dialog.removeEventListener("close", this.handleCloseEvent(reference));
        
     }

    private handleCloseEvent(reference) {
       return () => reference.invokeMethodAsync("Close");
    }
}

const testClass = new closeListener();

export function OpenDialog(guid) {
    var dialog = document.getElementById(guid) as HTMLDialogElement;
    dialog.showModal();
};

export function CloseDialog(guid, reference) {
    var dialog = document.getElementById(guid) as HTMLDialogElement;
    testClass.removeListener(guid, reference);
    dialog.close(); 
};

export function CloseListener(guid, reference) {
    testClass.addListener(guid, reference);
};