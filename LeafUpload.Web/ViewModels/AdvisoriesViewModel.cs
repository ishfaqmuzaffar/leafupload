using LeafUpload.Core.Models;

namespace LeafUpload.Web.ViewModels
{
    public class FarmAdvisoryViewModel
    {
        public Farm Farm { get; set; } = null!;
        public Advisory? Advisory { get; set; }
    }
}
