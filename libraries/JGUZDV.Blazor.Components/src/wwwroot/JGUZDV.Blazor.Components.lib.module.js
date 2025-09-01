export function registerThemeButtons() {
    document.querySelectorAll('[data-set-theme]').forEach(el => {
        el.addEventListener('click', (event) => {
            const target = event.currentTarget;
            const theme = target.getAttribute('data-set-theme');
            if (theme) {
                applyTheme(theme, false);
            }
            const dropdownMenu = target.closest('.dropdown-menu');
            if (dropdownMenu) {
                dropdownMenu.hidden = true;
                const buttonId = dropdownMenu.getAttribute('aria-labelledby');
                if (buttonId) {
                    const button = document.getElementById(buttonId);
                    if (button) {
                        button.setAttribute('aria-expanded', 'false');
                    }
                }
            }
        });
    });
    console.debug('theme buttons were registered');
}
export function applyTheme(theme, isInit) {
    if (theme === 'auto') {
        const preferred = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        document.documentElement.setAttribute('data-bs-theme', preferred);
        localStorage.removeItem("theme");
    }
    else {
        document.documentElement.setAttribute('data-bs-theme', theme);
        if (!isInit) {
            localStorage.setItem("theme", theme);
        }
    }
    console.debug('theme was set', theme);
    updateThemeIcon(theme);
    updateActiveThemeButton(theme);
}
export function setStoredTheme() {
    const stored = localStorage.getItem("theme");
    const theme = stored ?? 'light';
    applyTheme(theme, true);
    return theme;
}
export function updateThemeIcon(theme) {
    const iconElement = document.querySelector("#jgu-current-theme-icon i");
    if (!iconElement)
        return;
    const themeClass = {
        light: "fa-sun",
        dark: "fa-moon",
        auto: "fa-adjust"
    }[theme] ?? "fa-adjust";
    iconElement.className = `fas ${themeClass}`;
}
export function updateActiveThemeButton(theme) {
    const themes = ["light", "dark", "auto"];
    themes.forEach(t => {
        const btn = document.getElementById(`jgu-${t}-theme-icon`);
        if (btn) {
            btn.classList.toggle("active", t === theme);
        }
    });
}
export function registerWebComponents() {
    customElements.define('jgu-dropdown', class extends HTMLElement {
        constructor() {
            super();
        }
        connectedCallback() {
            const id = this.getAttribute('button-id');
            if (!id) {
                console.warn("No button-id provided for <jgu-dropdown>");
                return;
            }
            const button = document.getElementById(id);
            const menuId = button?.getAttribute('aria-controls');
            const menu = menuId ? document.getElementById(menuId) : null;
            if (!button || !menu) {
                console.error("Button or menu not found for <jgu-dropdown> :", button, menu);
                return;
            }
            button.addEventListener('click', (e) => {
                const shouldClose = button.getAttribute('aria-expanded') === 'true';
                button.setAttribute('aria-expanded', String(!shouldClose));
                menu.hidden = shouldClose;
                if (!shouldClose) {
                    menu.querySelector('[role="menuitem"]')?.focus();
                    document.addEventListener('click', this.globalCloseHandler);
                }
                else {
                    document.removeEventListener('click', this.globalCloseHandler);
                }
            });
            console.debug("The button and menu was connected: ", button, menu);
        }
        globalCloseHandler(event) {
            const id = this.getAttribute('button-id');
            const button = document.getElementById(id);
            const menuId = button?.getAttribute('aria-controls');
            const menu = document.getElementById(menuId);
            const target = event.target;
            console.debug("The globalCloseHandler was triggered", button);
            if (!button.contains(target) && !menu.contains(target)) {
                button.setAttribute('aria-expanded', 'false');
                menu.hidden = true;
            }
            document.removeEventListener('click', this.globalCloseHandler);
        }
    });
    console.debug('web components (jgu-dropdown) were registered');
    customElements.define('jbs-toggle', class extends HTMLElement {
        constructor() {
            super();
        }
        connectedCallback() {
            this.addEventListener("click", () => {
                const targetId = this.getAttribute('target-id');
                if (!targetId) {
                    console.warn("No target-id provided for <jbs-toggle>");
                    return;
                }
                const toggleClass = this.getAttribute('toggle-class') || "toggled";
                document.getElementById(targetId)?.classList.toggle(toggleClass);
            });
        }
    });
    console.debug('web components (jbs-toggle) were registered');
}
export function afterWebStarted(blazor) {
    registerThemeButtons();
}
export function beforeWebStart(options) {
    setStoredTheme();
    registerWebComponents();
}
