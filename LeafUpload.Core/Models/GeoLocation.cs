namespace LeafUpload.Core.Models
{
    public class GeoLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ResolvedName { get; set; } = string.Empty;
    }
}
