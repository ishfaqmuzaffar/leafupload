using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Persistence
{
    public class EfFarmerRepository : IFarmerRepository
    {
        private readonly LeafUploadDbContext _db;

        public EfFarmerRepository(LeafUploadDbContext db)
        {
            _db = db;
        }

        public async Task CreateFarmerWithFarmAsync(Farmer farmer, Farm farm)
        {
            farmer.Username = Normalize(farmer.Username);
            _db.Farmers.Add(farmer);

            farm.FarmerId = farmer.Id;
            _db.Farms.Add(farm);

            await _db.SaveChangesAsync();
        }

        public async Task AddFarmAsync(Farm farm)
        {
            _db.Farms.Add(farm);
            await _db.SaveChangesAsync();
        }

        public async Task<Farmer?> GetFarmerByUsernameAsync(string username)
        {
            var normalized = Normalize(username);
            return await _db.Farmers.FirstOrDefaultAsync(f => f.Username == normalized);
        }

        public async Task<Farmer?> GetFarmerByIdAsync(Guid id)
        {
            return await _db.Farmers.FindAsync(id);
        }

        public async Task<IReadOnlyList<Farm>> GetFarmsByFarmerIdAsync(Guid farmerId)
        {
            return await _db.Farms.Where(f => f.FarmerId == farmerId).ToListAsync();
        }

        public async Task<int> GetFarmerCountAsync()
        {
            return await _db.Farmers.CountAsync();
        }

        public async Task<IReadOnlyList<Farm>> GetAllFarmsAsync()
        {
            return await _db.Farms.ToListAsync();
        }

        private static string Normalize(string username) => username.Trim().ToLowerInvariant();
    }
}
