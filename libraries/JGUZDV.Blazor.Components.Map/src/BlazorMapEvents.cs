using Microsoft.AspNetCore.Components.Web;
using GeoJSON.Text.Feature;

namespace JGUZDV.Blazor.Components.Map
{
    public class MapMouseEventArgs
    {
        public MouseEventArgs? OriginalEvent { get; set; }
        public LngLat? LngLat { get; set; }
        public List<MapGeoJSONFeature> Features { get; set; } = new();
    }

    public class LngLat
    {
        public double Lng { get; set; }
        public double Lat { get; set; }

        public LngLat(double lng, double lat)
        {
            Lng = lng;
            Lat = lat;
        }   
    }


    public class MapGeoJSONFeature
    {
        public Feature? Feature { get; set; }
        public string? LayerId { get; set; }
        public string? SourceId { get; set; }
    }
}
