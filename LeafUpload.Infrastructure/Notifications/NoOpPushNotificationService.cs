using LeafUpload.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Notifications
{
    // Used when no Firebase service account credential is configured (e.g. a fresh
    // clone of the repo without firebase-service-account.json) - degrades gracefully
    // instead of the app failing to start, same pattern as ClaudeAdvisoryGenerator's
    // fallback when there's no Anthropic API key.
    public class NoOpPushNotificationService : IPushNotificationService
    {
        private readonly ILogger<NoOpPushNotificationService> _logger;

        public NoOpPushNotificationService(ILogger<NoOpPushNotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendToTokensAsync(IEnumerable<string> deviceTokens, string title, string body)
        {
            _logger.LogDebug("Push notifications not configured (no Firebase credential) - skipping send of '{Title}'.", title);
            return Task.CompletedTask;
        }
    }
}
