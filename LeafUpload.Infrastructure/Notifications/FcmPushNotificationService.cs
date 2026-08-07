using FirebaseAdmin.Messaging;
using LeafUpload.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Notifications
{
    // Requires FirebaseApp.Create(...) to have already been called at startup
    // (see Program.cs) - only registered when a service account credential is present.
    public class FcmPushNotificationService : IPushNotificationService
    {
        private readonly ILogger<FcmPushNotificationService> _logger;

        public FcmPushNotificationService(ILogger<FcmPushNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendToTokensAsync(IEnumerable<string> deviceTokens, string title, string body)
        {
            var tokens = deviceTokens.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
            if (tokens.Count == 0)
                return;

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification { Title = title, Body = body },
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                if (response.FailureCount > 0)
                {
                    _logger.LogWarning("FCM: {Failed}/{Total} push notifications failed to send.", response.FailureCount, tokens.Count);
                }
            }
            catch (System.Exception ex)
            {
                // A weather alert farmers don't get pushed is far less bad than a
                // crashed advisory refresh - log and move on.
                _logger.LogWarning(ex, "Failed to send push notification via FCM.");
            }
        }
    }
}
