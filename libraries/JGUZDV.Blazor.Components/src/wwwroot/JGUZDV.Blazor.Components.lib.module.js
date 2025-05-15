export function registerThemeButtons() {
    document.querySelectorAll('[data-set-theme]').forEach(el => {
        el.addEventListener('click', (event) => {
            const target = event.currentTarget;
            const theme = target.getAttribute('data-set-theme');
            if (theme) {
                applyTheme(theme, false);
            }
        });
    });
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
export function afterWebStarted() {
    setStoredTheme();
    registerThemeButtons();
}
document.addEventListener("DOMContentLoaded", () => {
    setStoredTheme();
    registerThemeButtons();
});
