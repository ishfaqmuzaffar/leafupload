using System;

namespace LeafUpload.Core.Models
{
    // An FCM registration token for one of a farmer's devices/browsers, used to
    // send push notifications for weather alerts.
    public class DeviceToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FarmerId { get; set; }
        public string Token { get; set; } = string.Empty;

        // "web", "android", or "ios".
        public string Platform { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
