// Typ-Deklaration für das DotNet-Objekt von Blazordeclare global {
    interface Window {
        DotNet: {
            createJSObjectReference(obj: any): any;
            getJSObjectReference(ref: any): any;
        };
    }
}
export function initEscapeHandler(popoverId: string): any {
    const handler = (e: KeyboardEvent): void => {
        if (e.key === 'Escape') {
            const popover = document.getElementById(popoverId);
            if (popover && popover.matches(':popover-open')) {
                (popover as HTMLElement & { hidePopover: () => void }).hidePopover();
            }
        }
    };
    document.addEventListener('keydown', handler);
    return window.DotNet.createJSObjectReference(handler);
}
export function disposeEscapeHandler(handlerRef: any): void {
    const handler = window.DotNet.getJSObjectReference(handlerRef);
    document.removeEventListener('keydown', handler);
}