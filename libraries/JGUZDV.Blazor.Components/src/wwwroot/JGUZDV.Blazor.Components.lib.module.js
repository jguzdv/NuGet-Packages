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
export function setupDropdown(id) {
    const button = document.getElementById(id);
    const menuId = button?.getAttribute('aria-controls');
    const menu = menuId ? document.getElementById(menuId) : null;
    if (!button || !menu)
        return;
    button.addEventListener('click', (e) => {
        const expanded = button.getAttribute('aria-expanded') === 'true';
        button.setAttribute('aria-expanded', String(!expanded));
        menu.hidden = expanded;
        if (!expanded) {
            menu.querySelector('[role="menuitem"]')?.focus();
        }
    });
    document.addEventListener('click', (event) => {
        const target = event.target;
        if (!button.contains(target) && !menu.contains(target)) {
            button.setAttribute('aria-expanded', 'false');
            menu.hidden = true;
        }
    });
    console.debug('dropdowns were set');
}
export function setupSidebarToggle() {
    const toggleBtn = document.getElementById("sidebar-toggle-btn");
    const sidebar = document.getElementById("jbs-sidebar");
    if (!toggleBtn || !sidebar)
        return;
    toggleBtn.addEventListener("click", () => {
        sidebar.classList.toggle("toggled");
    });
    document.querySelectorAll('[data-sidebar-close]').forEach(el => {
        el.addEventListener("click", () => {
            if (window.innerWidth < 992) {
                sidebar.classList.remove("toggled");
            }
        });
    });
    console.debug('sidebar toggle was set');
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
            setupDropdown(id);
        }
    });
    console.debug('web components (jgu-dropdown) were registered');
}
export function afterWebStarted(blazor) {
    registerThemeButtons();
    setupSidebarToggle();
}
export function beforeWebStarted(options) {
    setStoredTheme();
    registerWebComponents();
}
