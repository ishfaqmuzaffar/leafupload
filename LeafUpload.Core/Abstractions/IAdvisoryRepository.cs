using LeafUpload.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IAdvisoryRepository
    {
        Task SaveAdvisoryAsync(Advisory advisory);
        Task<Advisory?> GetLatestAdvisoryForFarmAsync(Guid farmId);

        // One (the most recent) advisory per farm, platform-wide - for the landing
        // page's "active weather risks" chart.
        Task<IReadOnlyList<Advisory>> GetLatestAdvisoriesForAllFarmsAsync();
    }
}
