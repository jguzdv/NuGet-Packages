export const beforeWebAssemblyStart = (options: any, extensions: any) => {
    document.documentElement.style.setProperty('--blazor-loader-display', 'block');
};

export const afterWebAssemblyStarted = (blazor: any) => {
    document.documentElement.style.setProperty('--blazor-loader-display', 'none');
}