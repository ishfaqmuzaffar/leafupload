namespace LeafUpload.Web.Models.Mobile
{
    public class CreateMobileFarmRequest
    {
        public string PlaceName { get; set; } = string.Empty;
        public string CropType { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ResolvedLocationName { get; set; }
    }

    public class MobileFarmDto
    {
        public Guid Id { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public string? ResolvedLocationName { get; set; }
        public string CropType { get; set; } = string.Empty;
        public string CropIcon { get; set; } = string.Empty;
        public string CropNameLocalized { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
