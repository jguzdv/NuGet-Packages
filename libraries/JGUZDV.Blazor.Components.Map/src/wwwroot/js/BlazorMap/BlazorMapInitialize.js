import { createMap } from "./BlazorMap.js"

let htmlwrapper = document.getElementById("jgu-zdv-blazormap-htmlwrapper");

let blazormapobjects = document.getElementById("jgu-zdv-blazormap-blazormapobjects").innerHTML;
blazormapobjects = JSON.parse(blazormapobjects);

let map = createMap(null, htmlwrapper, blazormapobjects.isStatic, blazormapobjects.center, blazormapobjects.zoom, blazormapobjects.baseLayerStyleUrl, blazormapobjects.maxBounds, blazormapobjects.spritePathPrefix || "");

for (let layer of blazormapobjects.additionalLayerStyleUrls) {
    await map.addStyleLayerFromUrl(layer);
    console.debug(`layerstyleurls`);
}

for (let key in blazormapobjects.additionalSourceData) {
    console.debug(`additionalSourceData`);
    map.setSourceData(key, blazormapobjects.additionalSourceData[key]);
}
