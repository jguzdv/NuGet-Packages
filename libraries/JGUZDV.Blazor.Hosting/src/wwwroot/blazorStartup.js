const applicationCulture = document.documentElement.getAttribute('blazor-culture') || 'de-DE';

Blazor.start({
    webAssembly: {
        applicationCulture: applicationCulture
    }
});