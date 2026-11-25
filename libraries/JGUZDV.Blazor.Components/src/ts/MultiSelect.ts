const navigationKeys = ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'];

export function preventArrowKeyScrolling(element: HTMLElement | null): void {
    console.log('preventArrowKeyScrolling registered', element);

    if (!element) {
        return;
    }

    element.addEventListener('keydown', (event: KeyboardEvent): void => {
        console.log('keydown on items container', event.key);

        if (navigationKeys.includes(event.key)) {
            event.preventDefault();
        }
    });
}

export function scrollToActiveItem(element: HTMLElement) {
    if (!element) return;

    const activeItem = element.querySelector(".keyboard-active");

    activeItem?.scrollIntoView({
        block: "nearest"
    })
}