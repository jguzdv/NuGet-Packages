export const showToastContainer = (containerId: string) => {
    const container = document.getElementById(containerId);
    if (container) {
        container.showPopover();
    }
}

export const hideToastContainer = (containerId: string) => {
    const container = document.getElementById(containerId);
    if (container) {
        container.hidePopover();
    }
}