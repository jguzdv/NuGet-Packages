
export class MultiSelect {
    public Component: any;
    public HtmlElement: HTMLElement;

    constructor(htmlElement: HTMLElement, component: any) {
        this.HtmlElement = htmlElement;
        this.Component = component;
    }
}

let multiSelect: MultiSelect | null = null;
export function registerMultiSelectListener(multiSelectComponent: any, htmlElement: HTMLElement): void {

    multiSelect = new MultiSelect(htmlElement, multiSelectComponent);
    document.addEventListener('click', globalCloseHandler);
}


const globalCloseHandler = (_event: Event): void => {
    let target = _event.target as Node

    if (multiSelect?.HtmlElement.contains(target)) {
        return;
    }
    
    multiSelect?.Component.invokeMethodAsync("ToggleSelection");
};

export function deregisterMultiSelectListener(): void {
    document.removeEventListener('click', globalCloseHandler);
}

