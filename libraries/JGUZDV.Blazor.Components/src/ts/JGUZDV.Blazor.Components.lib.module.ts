export function registerThemeButtons(): void {
    customElements.define('jgu-theme-button', class extends HTMLElement {
        connectedCallback(): void {
            this.addEventListener('click', this.handleClick);

            const currentTheme = localStorage.getItem("theme") ?? "auto";

            this.classList.toggle(
                "active",
                this.getAttribute("theme") === currentTheme
            );
        }

        disconnectedCallback(): void {
            this.removeEventListener('click', this.handleClick);
        }

        handleClick = (): void => {
            const theme = this.getAttribute('theme');
            if (!theme) return;

            applyTheme(theme);
            localStorage.setItem("theme", theme);
        };
    });

    console.debug('web component (jgu-theme-button) registered');

    customElements.define('jgu-theme-icon', class extends HTMLElement {
        observer?: MutationObserver;

        connectedCallback(): void {
            this.render();

            applyTheme(localStorage.getItem("theme") ?? "auto");

            this.observer = new MutationObserver(() => this.render());
            this.observer.observe(document.documentElement, {
                attributes: true,
                attributeFilter: ['data-bs-theme']
            });
        }

        disconnectedCallback(): void {
            this.observer?.disconnect();
        }

        render(): void {
            const theme = localStorage.getItem("theme") ?? "auto";
            const map: Record<string, string> = {
                light: "fa-sun",
                dark: "fa-moon",
                auto: "fa-adjust"
            };

            this.innerHTML = `<i class="fas ${map[theme] ?? "fa-adjust"}"></i>`;
        }
    });

    console.debug('web component (jgu-theme-icon) registered');
}

export function registerThemeGuard(): void {
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

export function applyTheme(theme: string): void {
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

export function setStoredTheme(): void {
    const stored = localStorage.getItem("theme");
    if (stored == null) {
        return;
    }
    applyTheme(stored);
}

export function registerWebComponents(): void {
    customElements.define('jgu-dropdown', class extends HTMLElement {
        connectedCallback(): void {
            requestAnimationFrame(() => {
                const button = this.querySelector('button');
                const menu = this.querySelector<HTMLElement>('[popover]');
                if (!button || !menu) return;

                menu.addEventListener('toggle', (event) => {
                    const toggleEvent = event as ToggleEvent;
                    button.setAttribute('aria-expanded', String(toggleEvent.newState === 'open'));

                    if (toggleEvent.newState === 'open') {
                        menu.querySelector<HTMLElement>('[role="menuitem"]')?.focus();
                    }
                });

                menu.addEventListener('click', (event) => {
                    if ((event.target as Element).closest('[role="menuitem"]')) {
                        menu.hidePopover();
                    }
                });
            });
            }
    });


    console.debug('web component (jgu-dropdown) registered');

    customElements.define('jgu-toggle', class extends HTMLElement {
        constructor() {
            super();
        }

        connectedCallback(): void {
            this.addEventListener("click", () => {
                const targetId = this.getAttribute('target-id');
                if (!targetId) {
                    console.warn("no target-id provided for <jgu-toggle>");
                    return;
                }

                const toggleClass =
                    this.getAttribute('toggle-class') || "toggled";

                document.getElementById(targetId)?.classList.toggle(toggleClass);
            });
        }
    });

    console.debug('web component (jgu-toggle) registered');
}

export function beforeWebStart(options?: unknown): void {
    registerWebComponents();
    registerThemeButtons();
    registerThemeGuard();
}
