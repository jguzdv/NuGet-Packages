class FullScreenAppLoader extends HTMLElement {
    constructor() {
        super();
    }
    connectedCallback() {
        this.render();
    }
    render = () => {
        this.innerHTML = `
            <style>
            .loading {
                width: 80dvw;
                background-color: #FFF;
                padding: 0.5rem;
                border: none
            }
            .loading::backdrop {
                background-color: rgba(35, 55, 60, 0.5);
                backdrop-filter: blur(4px);
            }

            .loading-progress:after {
                padding: 0.25rem;

                display: block;
                overflow: visible;
                background-color: #c10b25;

                width: var(--blazor-load-percentage, 0%);
                transition: width 0.2s ease-in-out;
                margin: 0 0 0 auto;

                color: #FFF;
                content: var(--blazor-load-percentage-text, "");
                text-align: center;
                font-weight: bold;
                text-shadow: 1px 1px 0 #23373c, -1px 1px 0 #23373c, 1px -1px 0 #23373c, -1px -1px 0 #23373c;
            }
            </style>
            <dialog class="loading">
                <div class="loading-progress"></div>
            </dialog> 
        `;
    };
}
class PrerenderAppLoader extends HTMLElement {
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

            .preloaded-loader-progress {
                  --percentage: var(--blazor-load-percentage);
                  animation: progress 2s 0.5s forwards;
                  width: 64px;
                  aspect-ratio: 1;
                  border-radius: 50%;
                  position: absolute;
                  bottom: 16px;
                  right: 16px;
                  overflow: hidden;
                  display: grid;
                  place-items: center;
            }

            .preloaded-loader-progress::before {
                  content: "";
                  position: absolute;
                  width: 100%;
                  height: 100%;
                  background: conic-gradient(#c10b25 var(--percentage), #23373c 0);
            }

            .preloaded-loader-progress::after {
                content: var(--blazor-load-percentage-text, "");
                position: absolute;

                color: #FFF;
                text-align: center;
                font-weight: bold;
                text-shadow: 1px 1px 0 #23373c, -1px 1px 0 #23373c, 1px -1px 0 #23373c, -1px -1px 0 #23373c;

                width: 75%;
                background: #FFF;
                aspect-ratio: 1;
                align-content: center;
                border-radius: 50%
            }
            </style>
            <div role="progressbar" class="preloaded-loader-progress">
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
    console.debug('Running beforeWebAssemblyStart');

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
    
    const appLoader = document.createElement('app-loader');
    appLoader.setAttribute('id', '__app-loader');
    appLoader.classList.add(document.documentElement.getAttribute("loader-type"));
    document.documentElement.appendChild(appLoader);
    if (document.documentElement.getAttribute("loader-type") == 'fullscreen') {
        appLoader.setAttribute('data-fullscreen', 'true');
        appLoader.getElementsByTagName('dialog')[0].showModal();
    }
};
const hideLoader = () => {
    const appLoader = document.getElementById('__app-loader');
    if (appLoader) {
        if (!!appLoader.getAttribute('data-fullscreen')) {
            appLoader.getElementsByTagName('dialog')[0].close();
        }
        document.documentElement.removeChild(appLoader);
    }
};
