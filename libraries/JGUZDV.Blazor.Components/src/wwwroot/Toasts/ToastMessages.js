export const showToastContainer = (containerId) => {
    const container = document.getElementById(containerId);
    if (container) {
        container.showPopover();
    }
};
export const hideToastContainer = (containerId) => {
    const container = document.getElementById(containerId);
    if (container) {
        container.hidePopover();
    }
};
