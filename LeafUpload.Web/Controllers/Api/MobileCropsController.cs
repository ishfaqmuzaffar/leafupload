using LeafUpload.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LeafUpload.Web.Controllers.Api
{
    // No auth needed - this is the same fixed 14-crop list Register.cshtml already shows
    // (CropTaxonomy.Crops), just exposed as JSON for the mobile app's crop dropdown.
    [ApiController]
    [Route("api/mobile/crops")]
    public class MobileCropsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCrops()
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var crops = CropTaxonomy.Crops.Select(crop => new
            {
                Name = crop,
                Icon = CropTaxonomy.IconFor(crop),
                Family = CropTaxonomy.FamilyFor(crop).ToString(),
                LocalizedName = CropTaxonomy.NameFor(crop, culture),
            });

            return Ok(crops);
        }
    }
}
