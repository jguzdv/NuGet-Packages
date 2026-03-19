import { createMap } from "./BlazorMap.js"

export async function initializeBlazorMap(mapElementId, objectsElementId) {
    let htmlwrapper = document.getElementById(mapElementId);
    let objectsElement = document.getElementById(objectsElementId);

    if (!htmlwrapper || !objectsElement) {
        console.error("BlazorMap initialization failed: required elements not found.");
        return;
    }

    let blazormapobjects;
    try {
        blazormapobjects = JSON.parse(objectsElement.textContent || "{}");
    } catch (error) {
        console.error("BlazorMap initialization failed: invalid JSON payload.", error);
        return;
    }

    let map = createMap(null, htmlwrapper, blazormapobjects.center, blazormapobjects.zoom, blazormapobjects.baseLayerStyleUrl, blazormapobjects.maxBounds, blazormapobjects.spritePathPrefix || "");

    for (let layer of blazormapobjects.additionalLayerStyleUrls || []) {
        await map.addStyleLayerFromUrl(layer);
    }

    let additionalSourceData = blazormapobjects.additionalSourceData || {};
    for (let key in additionalSourceData) {
        await map.setSourceData(key, additionalSourceData[key]);
    }
}
