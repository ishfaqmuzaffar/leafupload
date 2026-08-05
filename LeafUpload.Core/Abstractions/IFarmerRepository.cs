using LeafUpload.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IFarmerRepository
    {
        Task CreateFarmerWithFarmAsync(Farmer farmer, Farm farm);
        Task<Farmer?> GetFarmerByUsernameAsync(string username);
        Task<Farmer?> GetFarmerByIdAsync(Guid id);
        Task<IReadOnlyList<Farm>> GetFarmsByFarmerIdAsync(Guid farmerId);
    }
}
