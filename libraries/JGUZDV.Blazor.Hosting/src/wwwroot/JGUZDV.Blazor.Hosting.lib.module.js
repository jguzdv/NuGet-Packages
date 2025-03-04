class AppLoader extends HTMLElement {
    constructor() {
        super();
    }
    connectedCallback() {
        this.render();
    }
    render = () => {
        this.innerHTML = `
            <style>
            @keyframes progress {
                0% { --percentage: 0%; }
                100% { --percentage: var(--blazor-load-percentage); }
            }

            @property --percentage {
                syntax: '<percentage>';
                inherits: true;
                initial-value: 0%;
            }

            app-loader {
                z-index: 2147483645;

                &.fullscreen {
                    --width: min(38.2dvw, 38.2dvh);
                    --border-radius: 0;
                    width: var(--width);
                    margin-left: calc(-0.5 * var(--width));

                    position: absolute;
                    top: 10dvh;
                    left: 50%;
                }

                &.on-content {
                    width: 64px;
                    --border-radius: 50%;

                    position: absolute;
                    right: 16px; 
                    bottom: 16px;
                }

                .loader-progress {
                    --percentage: 0%;
                    position: relative;
                    animation: progress 2s 0.5s forwards;
                    width: 100%;
                    aspect-ratio: 1;
                    overflow: hidden;
                    display: grid;
                    place-items: center;
                }

                .loader-progress::before {
                    content: '';
                    position: absolute;
                    border-radius: var(--border-radius, 0);
                    width: 100%;
                    aspect-ratio: 1;
                    background: conic-gradient(#c10b25 var(--percentage), #23373c 0);
                }

                .loader-progress::after {
                    content: var(--blazor-load-percentage-text, "");
                    position: absolute;

                    color: #FFF;
                    text-align: center;
                    font-weight: bold;
                    color: #23373c;

                    width: calc(100% - 15px);
                    background: #FFF;
                    aspect-ratio: 1;
                    align-content: center;
                    border-radius: var(--border-radius, 0);
                }
            }
            </style>

            <div role="progressbar" class="loader-progress">
            </div>
        `;
    };
}

export const beforeWebStart = () => {
    console.debug('Running beforeWebStart');

    registerLoader();
};
export const afterWebStarted = () => {
    console.debug('Running afterWebStart');
};

export const beforeWebAssemblyStart = (options, extensions) => {
    console.debug('Running beforeWebAssemblyStart', options, extensions);

    setApplicaitonLanguage(options);

    showLoader();
};
export const afterWebAssemblyStarted = (blazor) => {
    console.debug('Running afterWebAssemblyStart');
    hideLoader();
};

const setApplicaitonLanguage = (opt) => {
    opt.applicationCulture = document.documentElement.getAttribute('blazor-culture') || 'de-DE';
}

const registerLoader = () => {
    if (customElements.get('app-loader')) {
        return;
    }

    const useFullscreenLoader = document.documentElement.getAttribute("loader-type") == 'fullscreen'
        ? true : false;
    customElements.define('app-loader', AppLoader); 
}

const showLoader = () => {
    const useLoader = !document.documentElement.getAttribute("no-loader");
    if (!useLoader) {
        return;
    }

    const appLoader = document.createElement('app-loader');
    appLoader.setAttribute('id', '__app-loader');
    appLoader.classList.add(document.documentElement.getAttribute("loader-type") || 'on-content');

    document.body.appendChild(appLoader);
};

const hideLoader = () => {
    const appLoader = document.getElementById('__app-loader');
    if (appLoader) {
        document.body.removeChild(appLoader);
    }
};
