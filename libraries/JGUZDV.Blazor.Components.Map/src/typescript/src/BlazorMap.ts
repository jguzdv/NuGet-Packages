import { GeoJSONSource, LngLatBounds, LngLatLike, Map as GlMap, MapMouseEvent } from "maplibre-gl";

class BlazorMap {

    private _isInitialized;
    private $isInitialized: (value: void | PromiseLike<void>) => void = () => { };
    private _pathPrefix: string;
    private _initializationCompleted: boolean = false;
    private _map: GlMap;


    constructor(
        private _dotnetRef,
        _rootElement: HTMLElement,
        private _mapId: string,
        isStatic: boolean,
        center: LngLatLike,
        zoom: number,
        baselayerurl: string,
        maxBounds: LngLatBounds,
        pathPrefix: string
    ) {
        this._isInitialized = new Promise<void>(resolve => this.$isInitialized = resolve);

        this._pathPrefix = pathPrefix;
        if (this._pathPrefix && this._pathPrefix.endsWith("/")) {
            this._pathPrefix = this._pathPrefix.substr(0, this._pathPrefix.length - 1);
        }

        let map: GlMap = new GlMap({
            container: _rootElement,
            center: center,
            zoom: zoom,
            style: baselayerurl,
            maxBounds: maxBounds,
        })

        if ( _dotnetRef) {
            map.on('click', this.onMapClick);
            map.on('dblclick', this.onMapClick);
            map.on('contextmenu', this.onMapClick);

        } else {
            map["scrollZoom"].disable();
            map["boxZoom"].disable();
            map["dragRotate"].disable();
            map["dragPan"].disable();
            map["keyboard"].disable();
            map["doubleClickZoom"].disable();
            map["touchZoomRotate"].disable();
        }


        map.on('load', () => {
            if (_dotnetRef) {
                this._dotnetRef.invokeMethodAsync("JSInitialized");
            }
            this.completeInitialization();
        });

        map.on('error', (e: any) => {
            if (this._initializationCompleted) {
                return;
    }

            if (_dotnetRef) {
                this._dotnetRef.invokeMethodAsync("JSInitializationFailed", e?.error?.message ?? "Map initialization failed.");
            }
            this.completeInitialization();
        });
        this._map = map;
    }

    private completeInitialization = () => {
        if (this._initializationCompleted) {
            return;
        }

        this._initializationCompleted = true;
        this.$isInitialized();
    }

    private getMap = () => { return this._map; }

    private onMapClick = (e: MapMouseEvent) => {
        let map: GlMap = this.getMap();

        let originalEvent = {
            detail: e.originalEvent.detail,
            screenX: e.originalEvent.screenX,
            screenY: e.originalEvent.screenY,
            clientX: e.originalEvent.clientX,
            clientY: e.originalEvent.clientY,
            offsetX: e.originalEvent.offsetX,
            offsetY: e.originalEvent.offsetY,
            pageX: e.originalEvent.pageX,
            pageY: e.originalEvent.pageY,
            ctrlKey: e.originalEvent.ctrlKey,
            altKey: e.originalEvent.altKey,
            shiftKey: e.originalEvent.shiftKey,
            metaKey: e.originalEvent.metaKey,
            button: e.originalEvent.button,
            type: e.originalEvent.type
        };

        let features = map.queryRenderedFeatures(e.point).map(feature => Object.assign({}, { feature: feature }, { layerId: feature.layer.id }, { sourceId: feature.source }));
        let eArgs = Object.assign({}, { originalEvent: originalEvent }, { lnglat: e.lngLat }, { features: features });

        this._dotnetRef.invokeMethodAsync("JSOnMapClicked", eArgs);

        return false;
    }

    public addStyleLayerFromUrl = async (url: string) => {
        await this._isInitialized;
        let map: GlMap = this.getMap();

        let response = await fetch(url);
        let jsonData = await response.json();

        if (jsonData.images) {
            for (let image of jsonData.images) {
                if (!image.id) {
                    console.warn("Image without 'id' can not be added!");
                    continue;
                }
                if (!image.imgUrl) {
                    console.warn(`Image definition ${image.id} is missing required properties. (Required properties: imgUrl).`);
                    continue;
                }

                map.loadImage(this.buildUrl(image.imgUrl), function (error, icon) {
                    if (error) {
                        console.error(`Image: ${image.id} could not load!`, error);
                    }
                    if (!map.hasImage(image.id)) {
                        try {
                            map.addImage(image.id, icon!);
                        } catch (error) {
                            console.error(`Image: ${image.id} could not be added!`, error);
                        }
                    }
                    else {
                        console.warn("Image" + image.id + " already added");
                    }
                });

                if (!image.id) {
                    console.warn("Image without 'id' can not be added!");
                }
                if (!image.imgUrl) {
                    console.warn(`Image definition ${image.id} is missing required properties. (Required properties: imgUrl).`);
                }
            }
        } else {
            console.debug("No images were found!");
        }

        if (jsonData.sprite) {
            for (let sprite of jsonData.sprite) {
                if (!sprite.id) {
                    console.warn("Sprite without 'id' can not be added!");
                    continue;
                }
                if (!sprite.url) {
                    console.warn(`Sprite definition ${sprite.id} is missing required properties. (Required properties: url).`);
                    continue;
                }
                try {
                    map.addSprite(sprite.id, this.buildUrl(sprite.url));
                } catch (error) {
                    console.error(`Sprite: ${sprite.id} could not be added!`, error);
                }
            }
        } else {
            console.debug("No sprites were found!");
        }

        if (jsonData.sources) {
            for (let key in jsonData.sources) {
                if (jsonData.sources[key].type != "geojson") {
                    console.warn("Type of source is not geojson and therefore not supported!");
                    continue;
                }

                if (!!jsonData.sources[key].data && typeof (jsonData.sources[key].data) == typeof ("")) {
                    jsonData.sources[key].data = this.buildUrl(jsonData.sources[key].data);
                }
                await this.setSourceData(key, jsonData.sources[key].data);
            }
        } else {
            console.debug("No sources were found!");
        }

        if (jsonData.layers) {
            for (let layer of jsonData.layers) {

                if (!layer.id) {
                    console.warn("Layer without 'id' can not be added!");
                    continue;
                }
                if (!layer.type || !layer.source) {
                    console.warn(`Layer definition ${layer.id} is missing required properties. (Required properties: type, source).`);
                    continue;
                }

                map.addLayer(layer);
            }
        } else {
            console.debug("No layers were found!");
        }

    }

    public setSourceData = async (id: string, data) => {
        await this._isInitialized;
        let map: GlMap = this.getMap();

        if (!data) {
            console.debug(`Source data for ${id} is missing and therefore created.`);
        }
        data = data || {
            "type": "FeatureCollection",
            "features": []
        }

        let source = map.getSource(id);
        if (source) {
            console.debug(`Setting source data for ${id}.`, source);
            (<GeoJSONSource>source).setData(data);
        }
        else {
            console.debug(`Adding source definition for ${id}.`);

            try {
                map.addSource(id, {
                    "type": "geojson",
                    "data": data
                });
            } catch (error) {
                console.error(`Source: ${id} could not be added!`, error);
            }
        }
    }

    public removeLayer = async (id: string) => {
        await this._isInitialized;
        let map: GlMap = this.getMap();

        let layer = map.getLayer(id);
        if (layer)
            map.removeLayer(id);
    }

    public addLayer = async (layer) => {
        await this._isInitialized;
        let map: GlMap = this.getMap();

        map.addLayer(layer);
    }

    public removeSource = async (id: string) => {
        await this._isInitialized;
        let map: GlMap = this.getMap();
        map.removeSource(id);
    }

    public pantoLocation = async (bounds: LngLatBounds) => {
        await this._isInitialized;
        let map: GlMap = this.getMap();
        map.fitBounds(bounds);
    }

    public Dispose = async () => {
        let map: GlMap = this.getMap();

        this.completeInitialization();

        if (!map) {
            return;
        }

        if (this._dotnetRef) {
            map.off('click', this.onMapClick);
            map.off('dblclick', this.onMapClick);
            map.off('contextmenu', this.onMapClick);
        }

        map.remove();
    }

    public buildUrl = (path: string) => {
        if (path.startsWith("http://") || path.startsWith("https://")) {
            return path;
        }

        if (path.startsWith("/")) {
            path = path.substr(1, path.length - 1);
        }
        return this._pathPrefix + "/" + path;
    }
}


export function createMap(_dotnetRef, _rootElement: HTMLElement, center: LngLatLike, zoom: number, baselayerurl: string, maxBounds: LngLatBounds, spritePathPrefix: string) {
    console.debug("Creating Map ...")
    return new BlazorMap(_dotnetRef, _rootElement, center, zoom, baselayerurl, maxBounds, spritePathPrefix);
}


