export function registerThemeButtons() {
    customElements.define('jgu-theme-button', class extends HTMLElement {
        connectedCallback() {
            this.addEventListener('click', this.handleClick);

            const currentTheme = document.documentElement.getAttribute("data-bs-theme") || "auto";
            this.classList.toggle("active", this.getAttribute("theme") === currentTheme);
        }

        disconnectedCallback() {
            this.removeEventListener('click', this.handleClick);
        }

        handleClick = () => {
            const theme = this.getAttribute('theme');
            if (!theme) return;

            applyTheme(theme);
            localStorage.setItem("theme", theme);
        }
    });
    console.debug('web component (jgu-theme-button) registered');

    customElements.define('jgu-theme-icon', class extends HTMLElement {
        connectedCallback() {
            this.render();
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
            const map = { light: "fa-sun", dark: "fa-moon", auto: "fa-adjust" };

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
    const stored = localStorage.getItem("theme") ?? 'auto';
    applyTheme(stored);
    return stored;
}
export function registerWebComponents() {
    customElements.define('jgu-dropdown', class extends HTMLElement {
        constructor() {
            super();
        }
        connectedCallback() {
            this.addEventListener('click', this.toggleHandler);
            console.debug("jgu-dropdown connected: ", this);
        }
        disconnectedCallback() {
            const button = this.getElementsByTagName("button")[0];
            button.removeEventListener('click', this.toggleHandler);
        }
        toggleHandler = (event) => {
            const button = this.getElementsByTagName("button")[0];
            const target = event.target;
            if (!button.contains(target)) {
                return;
            }
            const menu = this.getElementsByTagName("div")[0];
            console.debug("toggleHandler triggered", this);
            const shouldClose = button.getAttribute('aria-expanded') === 'true';
            button.setAttribute('aria-expanded', String(!shouldClose));
            menu.hidden = shouldClose;
            if (!shouldClose) {
                menu.querySelector('[role="menuitem"]')?.focus();
                setTimeout(() => document.addEventListener('click', this.globalCloseHandler), 0);
            }
            else {
                document.removeEventListener('click', this.globalCloseHandler);
            }
        };
        globalCloseHandler = (event) => {
            const button = this.getElementsByTagName("button")[0];
            const menu = this.getElementsByTagName("div")[0];
            console.debug("globalCloseHandler triggered", this);
            button.setAttribute('aria-expanded', 'false');
            menu.hidden = true;
            document.removeEventListener('click', this.globalCloseHandler);
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
    setStoredTheme();
    registerThemeButtons();
    registerThemeGuard();
}
