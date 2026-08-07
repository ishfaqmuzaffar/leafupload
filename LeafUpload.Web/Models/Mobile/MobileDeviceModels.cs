namespace LeafUpload.Web.Models.Mobile
{
    public class RegisterDeviceRequest
    {
        public string Token { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
    }
}
