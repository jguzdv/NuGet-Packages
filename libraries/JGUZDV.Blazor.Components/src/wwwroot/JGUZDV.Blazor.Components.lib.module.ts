export type Theme = 'light' | 'dark' | 'auto';

export function applyTheme(theme: Theme): void {
    if (theme === 'auto') {
        const preferred = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        document.documentElement.setAttribute('data-bs-theme', preferred);
    } else {
        document.documentElement.setAttribute('data-bs-theme', theme);
    }
    localStorage.setItem("theme", theme);
    updateActiveThemeButton(theme);
}

export function loadTheme(): Theme {
    const stored = localStorage.getItem("theme") as Theme | null;
    const theme: Theme = stored ?? 'auto';

    if (theme === 'auto') {
        const preferred = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        document.documentElement.setAttribute('data-bs-theme', preferred);
    } else {
        document.documentElement.setAttribute('data-bs-theme', theme);
    }

    updateActiveThemeButton(theme);
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
    const theme = loadTheme();
    if ((window as any).blazor?.addEventListener) {
        (window as any).blazor.addEventListener("enhancedload", () => {
            loadTheme();
        });
    }
    updateThemeIcon(localStorage.getItem("theme") as Theme ?? "auto");
}

declare global {
    interface Window {
        JGUZDVBlazorComponents: {
            loadTheme: () => Theme;
            applyTheme: (theme: Theme) => void;
        };
    }
}

window.JGUZDVBlazorComponents = window.JGUZDVBlazorComponents || {};

window.JGUZDVBlazorComponents.loadTheme = loadTheme;
window.JGUZDVBlazorComponents.applyTheme = function (theme: Theme): void {
    applyTheme(theme);
    updateThemeIcon(theme);
};
