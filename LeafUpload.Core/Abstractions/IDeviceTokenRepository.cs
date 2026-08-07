using LeafUpload.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IDeviceTokenRepository
    {
        // Inserts a new token, or refreshes UpdatedAt/FarmerId/Platform if this exact
        // token already exists (tokens can be reassigned if a device/browser is reused
        // by a different farmer after a logout/login).
        Task UpsertAsync(Guid farmerId, string token, string platform);

        Task<IReadOnlyList<DeviceToken>> GetTokensForFarmerAsync(Guid farmerId);
    }
}
