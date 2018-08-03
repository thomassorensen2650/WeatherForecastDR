namespace WeatherForecastDR
{
    /// <summary>
    /// Model of a GeoSearchItem returned by Google GeoLocation Webservice
    /// </summary>
    public class GeoSearchItem
    {
        public string Address { get; set; }
        public float LocationLat { get; set; }
        public float LocationLng { get; set; }
    }
}
