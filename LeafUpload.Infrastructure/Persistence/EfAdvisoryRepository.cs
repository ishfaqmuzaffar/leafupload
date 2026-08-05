using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Persistence
{
    public class EfAdvisoryRepository : IAdvisoryRepository
    {
        private readonly LeafUploadDbContext _db;

        public EfAdvisoryRepository(LeafUploadDbContext db)
        {
            _db = db;
        }

        public async Task SaveAdvisoryAsync(Advisory advisory)
        {
            _db.Advisories.Add(advisory);
            await _db.SaveChangesAsync();
        }

        public async Task<Advisory?> GetLatestAdvisoryForFarmAsync(Guid farmId)
        {
            return await _db.Advisories
                .Where(a => a.FarmId == farmId)
                .OrderByDescending(a => a.GeneratedAt)
                .FirstOrDefaultAsync();
        }
    }
}
