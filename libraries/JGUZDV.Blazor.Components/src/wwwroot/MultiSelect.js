export class MultiSelect {
    Component;
    HtmlElement;
    constructor(htmlElement, component) {
        this.HtmlElement = htmlElement;
        this.Component = component;
    }
}
let multiSelect = null;
export function registerMultiSelectListener(multiSelectComponent, htmlElement) {
    multiSelect = new MultiSelect(htmlElement, multiSelectComponent);
    document.addEventListener('click', globalCloseHandler);
}
const globalCloseHandler = (_event) => {
    let target = _event.target;
    if (multiSelect?.HtmlElement.contains(target)) {
        return;
    }
    multiSelect?.Component.invokeMethodAsync("ToggleSelection");
};
export function deregisterMultiSelectListener() {
    document.removeEventListener('click', globalCloseHandler);
}
