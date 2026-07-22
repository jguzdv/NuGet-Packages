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