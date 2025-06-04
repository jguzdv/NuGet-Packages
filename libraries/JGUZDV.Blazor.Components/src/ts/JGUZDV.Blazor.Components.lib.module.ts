export type Theme = 'light' | 'dark' | 'auto';

export function registerThemeButtons(): void {
    document.querySelectorAll<HTMLElement>('[data-set-theme]').forEach(el => {
        el.addEventListener('click', (event: Event) => {
            const target = event.currentTarget as HTMLElement;
            const theme = target.getAttribute('data-set-theme') as Theme | null;
            if (theme) {
                applyTheme(theme, false);
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

export function setupDropdownCloseOnClick(): void {
    document.addEventListener('click', (event: MouseEvent) => {
        const target = event.target as Node;

        document.querySelectorAll<HTMLInputElement>('input.dropdown-toggle').forEach((checkbox) => {
            const dropdown = checkbox.closest('.dropdown');
            if (!dropdown) return;

            if (!dropdown.contains(target)) {
                checkbox.checked = false;
            }
        });
    });

    document.querySelectorAll<HTMLElement>('[data-set-theme]').forEach((el) => {
        el.addEventListener('click', () => {
            const dropdown = el.closest('.dropdown');
            const checkbox = dropdown?.querySelector<HTMLInputElement>('input.dropdown-toggle');
            if (checkbox) {
                checkbox.checked = false;
            }
        });
    });
}


export function afterWebStarted(): void {
    setStoredTheme();
    registerThemeButtons();
    setupDropdownCloseOnClick();
}
