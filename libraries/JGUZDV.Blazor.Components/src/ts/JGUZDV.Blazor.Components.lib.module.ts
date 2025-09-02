export type Theme = 'light' | 'dark' | 'auto';

export function registerThemeButtons(): void {
    document.querySelectorAll<HTMLElement>('[data-set-theme]').forEach(el => {
        el.addEventListener('click', (event: Event) => {
            const target = event.currentTarget as HTMLElement;
            const theme = target.getAttribute('data-set-theme') as Theme | null;
            if (theme) {
                applyTheme(theme, false);
            }

            const dropdownMenu = target.closest('.dropdown-menu') as HTMLElement;
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

    console.debug('theme buttons registered');
}

export function applyTheme(theme: Theme, isInit: Boolean): void {
    if (theme === 'auto') {
        const preferred = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        document.documentElement.setAttribute('data-bs-theme', preferred);
        localStorage.removeItem("theme");
    } else {
        document.documentElement.setAttribute('data-bs-theme', theme);
        if (!isInit) {
            localStorage.setItem("theme", theme);
        }
    }

    console.debug('theme set to: ', theme);

    updateThemeIcon(theme);
    updateActiveThemeButton(theme);
}

export function setStoredTheme(): Theme {
    const stored = localStorage.getItem("theme") as Theme | null;
    const theme: Theme = stored ?? 'light';

    applyTheme(theme, true);
    return theme;
}

export function updateThemeIcon(theme: Theme): void {
    const iconElement = document.querySelector<HTMLElement>("#jgu-current-theme-icon i");
    if (!iconElement) return;

    const themeClass = {
        light: "fa-sun",
        dark: "fa-moon",
        auto: "fa-adjust"
    }[theme] ?? "fa-adjust";

    iconElement.className = `fas ${themeClass}`;
}

export function updateActiveThemeButton(theme: Theme): void {
    const themes: Theme[] = ["light", "dark", "auto"];
    themes.forEach(t => {
        const btn = document.getElementById(`jgu-${t}-theme-icon`);
        if (btn) {
            btn.classList.toggle("active", t === theme);
        }
    });
}

export function registerWebComponents(): void {
    customElements.define('jgu-dropdown',
        class extends HTMLElement {
            constructor() {
                super();
            }

            connectedCallback() {
                const button = this.getElementsByTagName("button")[0];

                button.addEventListener('click', this.toggleHandler);

                console.debug("jgu-dropdown connected: ", this);
            }

            disconnectedCallback() {
                const button = this.getElementsByTagName("button")[0];
                button.removeEventListener('click', this.toggleHandler);
            }

            toggleHandler = (event: MouseEvent) => {
                const button = this.getElementsByTagName("button")[0];
                const menu = this.getElementsByTagName("div")[0];

                console.debug("toggleHandler triggered", this);

                const shouldClose = button.getAttribute('aria-expanded') === 'true';
                button.setAttribute('aria-expanded', String(!shouldClose));
                menu.hidden = shouldClose;

                if (!shouldClose) {
                    menu.querySelector<HTMLElement>('[role="menuitem"]')?.focus();
                    setTimeout(() => document.addEventListener('click', this.globalCloseHandler), 0);
                } else {
                    document.removeEventListener('click', this.globalCloseHandler);
                }
            }

            globalCloseHandler = (event: MouseEvent) => {
                const button = this.getElementsByTagName("button")[0];
                const menu = this.getElementsByTagName("div")[0];

                console.debug("globalCloseHandler triggered", this);

                button.setAttribute('aria-expanded', 'false');
                menu.hidden = true;

                document.removeEventListener('click', this.globalCloseHandler);
            }
        }
    );

    console.debug('web component (jgu-dropdown) registered');

    customElements.define('jgu-toggle',
        class extends HTMLElement {
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
        }
    );

    console.debug('web component (jgu-toggle) registered');
}

export function afterWebStarted(blazor: any): void {
    registerThemeButtons();
}

export function beforeWebStart(options: any): void {
    setStoredTheme();
    registerWebComponents();
} 