
using GeoJSON.Text.Feature;

namespace JGUZDV.Blazor.Components.Map
{
    internal class BlazorMapObjects
    {
        public LngLat Center { get; set; }
        public int Zoom { get; set; }
        public List<LngLat>? MaxBounds { get; set; }
        public bool IsStatic { get; set; }
        public string BaseLayerStyleUrl { get; set; }
        public IEnumerable<string>? AdditionalLayerStyleUrls { get; set; }
        public Dictionary<string, FeatureCollection> AdditionalSourceData { get; set; }
        public string SpritePathPrefix { get; set; }


        public BlazorMapObjects(bool isStatic, LngLat center, int zoom, string baseLayerStyleUrl, List<LngLat>? maxBounds, IEnumerable<string>? additionalLayerStyleUrls, Dictionary<string, FeatureCollection> additionalSourceData, string spritePathPrefix)
        {
            IsStatic = isStatic;
            Center = center;
            Zoom = zoom;
            BaseLayerStyleUrl = baseLayerStyleUrl;
            MaxBounds = maxBounds;
            AdditionalLayerStyleUrls = additionalLayerStyleUrls;
            AdditionalSourceData = additionalSourceData;
            SpritePathPrefix = spritePathPrefix;
        }
    }
}
