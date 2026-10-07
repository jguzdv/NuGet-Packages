export function registerThemeButtons() {
    if (customElements.get('jgu-theme-button'))
        return;
    customElements.define('jgu-theme-button', class extends HTMLElement {
        connectedCallback() {
            this.addEventListener('click', this.handleClick);
            const currentTheme = localStorage.getItem("theme") ?? "auto";
            this.classList.toggle("active", this.getAttribute("theme") === currentTheme);
        }
        disconnectedCallback() {
            this.removeEventListener('click', this.handleClick);
        }
        handleClick = () => {
            const theme = this.getAttribute('theme');
            if (!theme)
                return;
            applyTheme(theme);
            localStorage.setItem("theme", theme);
        };
    });
    console.debug('web component (jgu-theme-button) registered');
    customElements.define('jgu-theme-icon', class extends HTMLElement {
        observer;
        connectedCallback() {
            this.render();
            applyTheme(localStorage.getItem("theme") ?? "auto");
            this.observer = new MutationObserver(() => this.render());
            this.observer.observe(document.documentElement, {
                attributes: true,
                attributeFilter: ['data-bs-theme']
            });
        }
        disconnectedCallback() {
            this.observer?.disconnect();
        }
        render() {
            const theme = localStorage.getItem("theme") ?? "auto";
            const map = {
                light: "fa-sun",
                dark: "fa-moon",
                auto: "fa-adjust"
            };
            this.innerHTML = `<i class="fas ${map[theme] ?? "fa-adjust"}"></i>`;
        }
    });
    console.debug('web component (jgu-theme-icon) registered');
}
export function registerThemeGuard() {
    const observer = new MutationObserver(() => {
        if (!document.documentElement.hasAttribute("data-bs-theme")) {
            setStoredTheme();
        }
    });
    observer.observe(document.documentElement, {
        attributes: true,
        attributeFilter: ["data-bs-theme"]
    });
}
export function applyTheme(theme) {
    const isAuto = theme === 'auto';
    const resolved = isAuto
        ? (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light')
        : theme;
    document.documentElement.setAttribute('data-bs-theme', resolved);
    document.querySelectorAll("jgu-theme-button").forEach(btn => {
        btn.classList.toggle("active", btn.getAttribute("theme") === theme);
    });
    console.debug('theme is set to: ', theme);
}
export function setStoredTheme() {
    const stored = localStorage.getItem("theme");
    if (stored == null) {
        return;
    }
    applyTheme(stored);
}
export function registerWebComponents() {
    if (customElements.get('jgu-dropdown'))
        return;
    customElements.define('jgu-dropdown', class extends HTMLElement {
        connectedCallback() {
            this.addEventListener('click', this.clickHandler);
            this.addEventListener('toggle', this.toggleHandler, true);
        }
        disconnectedCallback() {
            this.removeEventListener('click', this.clickHandler);
            this.removeEventListener('toggle', this.toggleHandler, true);
        }
        clickHandler = (event) => {
            const target = event.target;
            if (!(target instanceof Element))
                return;
            if (!target.closest('[role="menuitem"]'))
                return;
            const menu = this.querySelector('[popover]');
            menu?.hidePopover();
        };
        toggleHandler = (event) => {
            const target = event.target;
            if (!(target instanceof Element))
                return;
            if (!target.hasAttribute('popover'))
                return;
            const toggleEvent = event;
            const button = this.querySelector('button');
            if (!button)
                return;
            button.setAttribute('aria-expanded', String(toggleEvent.newState === 'open'));
            if (toggleEvent.newState === 'open') {
                target.querySelector('[role="menuitem"]')?.focus();
            }
        };
    });
    console.debug('web component (jgu-dropdown) registered');
    customElements.define('jgu-toggle', class extends HTMLElement {
        constructor() {
            super();
        }
        connectedCallback() {
            this.addEventListener("click", () => {
                const targetId = this.getAttribute('target-id');
                if (!targetId) {
                    console.warn("no target-id provided for <jgu-toggle>");
                    return;
                }
                const toggleClass = this.getAttribute('toggle-class') || "toggled";
                document.getElementById(targetId)?.classList.toggle(toggleClass);
            });
        }
    });
    console.debug('web component (jgu-toggle) registered');
}
export function beforeWebStart(options) {
    registerWebComponents();
    registerThemeButtons();
    registerThemeGuard();
}
