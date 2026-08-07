namespace LeafUpload.Web.Models.Mobile
{
    public class MobileRegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
        public string CropType { get; set; } = string.Empty;

        // Optional - if the app already has device GPS coordinates it skips geocoding,
        // same fallback rule AccountController.Register uses for the web form.
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ResolvedLocationName { get; set; }
    }

    public class MobileLoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class MobileAuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public Guid FarmerId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
