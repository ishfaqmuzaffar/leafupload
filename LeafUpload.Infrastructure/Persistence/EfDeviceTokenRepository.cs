using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Persistence
{
    public class EfDeviceTokenRepository : IDeviceTokenRepository
    {
        private readonly LeafUploadDbContext _db;

        public EfDeviceTokenRepository(LeafUploadDbContext db)
        {
            _db = db;
        }

        public async Task UpsertAsync(Guid farmerId, string token, string platform)
        {
            var existing = await _db.DeviceTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (existing != null)
            {
                existing.FarmerId = farmerId;
                existing.Platform = platform;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _db.DeviceTokens.Add(new DeviceToken
                {
                    FarmerId = farmerId,
                    Token = token,
                    Platform = platform,
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<DeviceToken>> GetTokensForFarmerAsync(Guid farmerId)
        {
            return await _db.DeviceTokens.Where(t => t.FarmerId == farmerId).ToListAsync();
        }
    }
}
