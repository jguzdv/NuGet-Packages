var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { Map as GlMap } from "maplibre-gl";
class BlazorMap {
    constructor(_dotnetRef, _rootElement, _mapId, center, zoom, baselayerurl, maxBounds, pathPrefix) {
        this._dotnetRef = _dotnetRef;
        this._mapId = _mapId;
        this.$isInitialized = () => { };
        this.setMap = (map) => { window[this._mapId] = map; };
        this.getMap = () => { return window[this._mapId]; };
        this.onMapClick = (e) => {
            let map = this.getMap();
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
        };
        this.addStyleLayerFromUrl = (url) => __awaiter(this, void 0, void 0, function* () {
            yield this._isInitialized;
            let map = this.getMap();
            let response = yield fetch(url);
            let jsonData = yield response.json();
            if (jsonData.images) {
                for (let image of jsonData.images) {
                    map.loadImage(this.buildUrl(image.imgUrl), function (error, icon) {
                        if (error) {
                            console.error(`Image: ${image.id} could not load!`, error);
                        }
                        if (!map.hasImage(image.id)) {
                            try {
                                map.addImage(image.id, icon);
                            }
                            catch (error) {
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
            }
            else {
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
                    }
                    catch (error) {
                        console.error(`Sprite: ${sprite.id} could not be added!`, error);
                    }
                }
            }
            else {
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
                    yield this.setSourceData(key, jsonData.sources[key].data);
                }
            }
            else {
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
            }
            else {
                console.debug("No layers were found!");
            }
        });
        this.setSourceData = (id, data) => __awaiter(this, void 0, void 0, function* () {
            yield this._isInitialized;
            let map = this.getMap();
            if (!data) {
                console.debug(`Source data for ${id} is missing and therefore created.`);
            }
            data = data || {
                "type": "FeatureCollection",
                "features": []
            };
            let source = map.getSource(id);
            if (source) {
                console.debug(`Setting source data for ${id}.`, source);
                source.setData(data);
            }
            else {
                console.debug(`Adding source definition for ${id}.`);
                try {
                    map.addSource(id, {
                        "type": "geojson",
                        "data": data
                    });
                }
                catch (error) {
                    console.error(`Source: ${id} could not be added!`, error);
                }
            }
        });
        this.removeLayer = (id) => __awaiter(this, void 0, void 0, function* () {
            yield this._isInitialized;
            let map = this.getMap();
            let layer = map.getLayer(id);
            if (layer)
                map.removeLayer(id);
        });
        this.addLayer = (layer) => __awaiter(this, void 0, void 0, function* () {
            yield this._isInitialized;
            let map = this.getMap();
            map.addLayer(layer);
        });
        this.removeSource = (id) => __awaiter(this, void 0, void 0, function* () {
            yield this._isInitialized;
            let map = this.getMap();
            map.removeSource(id);
        });
        this.pantoLocation = (bounds) => __awaiter(this, void 0, void 0, function* () {
            yield this._isInitialized;
            let map = this.getMap();
            map.fitBounds(bounds);
        });
        this.buildUrl = (path) => {
            if (path.startsWith("http://") || path.startsWith("https://")) {
                return path;
            }
            if (path.startsWith("/")) {
                path = path.substr(1, path.length - 1);
            }
            return this._pathPrefix + "/" + path;
        };
        this._isInitialized = new Promise(resolve => this.$isInitialized = resolve);
        this._pathPrefix = pathPrefix;
        if (this._pathPrefix && this._pathPrefix.endsWith("/")) {
            this._pathPrefix = this._pathPrefix.substr(0, this._pathPrefix.length - 1);
        }
        let map = new GlMap({
            container: _rootElement,
            center: center,
            zoom: zoom,
            style: baselayerurl,
            maxBounds: maxBounds,
        });
        if (_dotnetRef) {
            map.on('click', this.onMapClick);
            map.on('dblclick', this.onMapClick);
            map.on('contextmenu', this.onMapClick);
        }
        map.on('load', () => {
            if (_dotnetRef) {
                this._dotnetRef.invokeMethodAsync("JSInitialized");
            }
            this.$isInitialized();
        });
        this.setMap(map);
    }
}
export function createMap(_dotnetRef, _rootElement, center, zoom, baselayerurl, maxBounds, spritePathPrefix) {
    console.debug("Creating Map ...");
    return new BlazorMap(_dotnetRef, _rootElement, `map_${Math.random()}`, center, zoom, baselayerurl, maxBounds, spritePathPrefix);
}

//# sourceMappingURL=BlazorMap.js.map