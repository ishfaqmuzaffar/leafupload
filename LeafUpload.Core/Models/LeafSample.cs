using System;

namespace LeafUpload.Core.Models
{
    public class LeafSample
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // File name of the uploaded image
        public string FileName { get; set; } = string.Empty;

        // Path where the file is stored (optional if needed later)
        public string FilePath { get; set; } = string.Empty;

        // When the file was uploaded
        public DateTime UploadedAt { get; set; }
    }
}
