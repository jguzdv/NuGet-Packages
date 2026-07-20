export function initEscapeHandler(popoverId) {
    const handler = (e) => {
        if (e.key === 'Escape') {
            const popover = document.getElementById(popoverId);
            if (popover && popover.matches(':popover-open')) {
                popover.hidePopover();
            }
        }
    };
    document.addEventListener('keydown', handler);
    return DotNet.createJSObjectReference(handler);
}

export function disposeEscapeHandler(handlerRef) {
    const handler = DotNet.getJSObjectReference(handlerRef);
    document.removeEventListener('keydown', handler);
}