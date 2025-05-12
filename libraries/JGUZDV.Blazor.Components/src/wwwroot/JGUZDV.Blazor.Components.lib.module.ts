export type Theme = 'light' | 'dark' | 'auto';

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


export function afterWebStarted(): void {
    setStoredTheme();
}

declare global {
    interface Window {
        JGUZDVBlazorComponents: {
            loadTheme: () => Theme;
            applyTheme: (theme: Theme, isInit: Boolean) => void;
        };
    }
}

window.JGUZDVBlazorComponents = {
    loadTheme : setStoredTheme,
    applyTheme: function (theme: Theme, isInit: Boolean): void {
        applyTheme(theme, isInit);
    }
};
