using LeafUpload.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IFarmerRepository
    {
        Task CreateFarmerWithFarmAsync(Farmer farmer, Farm farm);

        // Adds an additional farm to an already-existing farmer (e.g. the mobile app's
        // "add another farm" flow) - CreateFarmerWithFarmAsync always inserts a new
        // farmer row too, so it can't be reused for this.
        Task AddFarmAsync(Farm farm);
        Task<Farmer?> GetFarmerByUsernameAsync(string username);
        Task<Farmer?> GetFarmerByIdAsync(Guid id);
        Task<IReadOnlyList<Farm>> GetFarmsByFarmerIdAsync(Guid farmerId);

        // Platform-wide reads for the public landing page's stats/charts - not scoped
        // to a single farmer like the methods above.
        Task<int> GetFarmerCountAsync();
        Task<IReadOnlyList<Farm>> GetAllFarmsAsync();
    }
}
