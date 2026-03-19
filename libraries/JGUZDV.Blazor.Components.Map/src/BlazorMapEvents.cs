using Microsoft.AspNetCore.Components.Web;
using GeoJSON.Text.Feature;

namespace JGUZDV.Blazor.Components.Map
{
    /// <summary>
    /// Represents mouse event data raised by the map.
    /// </summary>
    public class MapMouseEventArgs
    {
        /// <summary>
        /// Gets or sets the original browser mouse event.
        /// </summary>
        public MouseEventArgs? OriginalEvent { get; set; }

        /// <summary>
        /// Gets or sets the geographic position of the mouse event.
        /// </summary>
        public LngLat? LngLat { get; set; }

        /// <summary>
        /// Gets or sets the rendered map features at the mouse position.
        /// </summary>
        public List<MapGeoJSONFeature> Features { get; set; } = [];
    }

    /// <summary>
    /// Represents a geographic coordinate as longitude and latitude.
    /// </summary>
    public class LngLat
    {
        /// <summary>
        /// Gets or sets the longitude value.
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// Gets or sets the latitude value.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LngLat"/> class.
        /// </summary>
        /// <param name="lng">The longitude value.</param>
        /// <param name="lat">The latitude value.</param>
        public LngLat(double lng, double lat)
        {
            Lng = lng;
            Lat = lat;
        }   
    }


    /// <summary>
    /// Represents a GeoJSON feature returned from a rendered map layer.
    /// </summary>
    public class MapGeoJSONFeature
    {
        /// <summary>
        /// Gets or sets the GeoJSON feature payload.
        /// </summary>
        public Feature? Feature { get; set; }

        /// <summary>
        /// Gets or sets the layer identifier of the feature.
        /// </summary>
        public string? LayerId { get; set; }

        /// <summary>
        /// Gets or sets the source identifier of the feature.
        /// </summary>
        public string? SourceId { get; set; }
    }
}
