const navigationKeys = ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'];
export function preventArrowKeyScrolling(element) {
    console.log('preventArrowKeyScrolling registered', element);
    if (!element) {
        return;
    }
    element.addEventListener('keydown', (event) => {
        console.log('keydown on items container', event.key);
        if (navigationKeys.includes(event.key)) {
            event.preventDefault();
        }
    });
}
export function scrollToActiveItem(element) {
    if (!element)
        return;
    const activeItem = element.querySelector(".keyboard-active");
    activeItem?.scrollIntoView({
        block: "nearest"
    });
}
