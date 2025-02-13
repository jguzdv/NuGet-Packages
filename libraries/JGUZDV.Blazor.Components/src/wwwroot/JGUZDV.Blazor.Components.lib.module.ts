class AppLoader extends HTMLElement {
    //private shadow: ShadowRoot;

    constructor() {
        super();
        //this.shadow = this.attachShadow({ mode: 'open' });
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

            .loading-progress {
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
    }
}


export const beforeWebAssemblyStart = (options: any, extensions: any) => {
    const useLoader = !document.documentElement.getAttribute("no-loader");

    if (!useLoader) {
        return;
    }

    customElements.define('app-loader', AppLoader);

    const appLoader = document.createElement('app-loader');
    appLoader.setAttribute('id', '__app-loader');

    document.documentElement.appendChild(appLoader);

    appLoader.getElementsByTagName('dialog')[0].showModal()
};

export const afterWebAssemblyStarted = (blazor: any) => {
    const appLoader = document.getElementById('__app-loader');
    if (appLoader) {
        appLoader.getElementsByTagName('dialog')[0].close();
        document.documentElement.removeChild(appLoader);
    }
}