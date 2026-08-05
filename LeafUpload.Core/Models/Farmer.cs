using System;

namespace LeafUpload.Core.Models
{
    public class Farmer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Stored lowercase; login lookups normalize to match.
        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
