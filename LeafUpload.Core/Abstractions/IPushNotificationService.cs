using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IPushNotificationService
    {
        Task SendToTokensAsync(IEnumerable<string> deviceTokens, string title, string body);
    }
}
