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

export function setupDropdowns(): void {
    const buttons = document.querySelectorAll<HTMLButtonElement>('[id^="dropdown-button-"]');

    buttons.forEach((button) => {
        const menuId = button.getAttribute('aria-controls');
        if (!menuId) return;

        const menu = document.getElementById(menuId) as HTMLElement | null;
        if (!menu) return;

        button.addEventListener('click', (e: MouseEvent) => {
            const expanded = button.getAttribute('aria-expanded') === 'true';
            button.setAttribute('aria-expanded', String(!expanded));
            menu.hidden = expanded;

            if (!expanded) {
                const firstItem = menu.querySelector<HTMLElement>('[role="menuitem"]');
                if (firstItem) {
                    firstItem.focus();
                }
            }
        });

        document.addEventListener('click', (event: MouseEvent) => {
            const target = event.target as Node;

            if (!button.contains(target) && !menu.contains(target)) {
                button.setAttribute('aria-expanded', 'false');
                menu.hidden = true;
            }
        });
    });
}

export function setupSidebarToggle() {
    const toggleBtn = document.getElementById("sidebar-toggle-btn");
    const sidebar = document.getElementById("jbs-sidebar");

    if (!toggleBtn || !sidebar) return;

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
}


export function afterWebStarted(): void {
    setStoredTheme();
    registerThemeButtons();
    setupDropdowns();
    setupSidebarToggle();
}
