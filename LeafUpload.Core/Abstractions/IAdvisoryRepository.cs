using LeafUpload.Core.Models;
using System;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IAdvisoryRepository
    {
        Task SaveAdvisoryAsync(Advisory advisory);
        Task<Advisory?> GetLatestAdvisoryForFarmAsync(Guid farmId);
    }
}
