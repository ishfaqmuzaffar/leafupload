using System.ComponentModel.DataAnnotations;

namespace LeafUpload.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Choose a username.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3-50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choose a password.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm your password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your farm's location.")]
        [Display(Name = "Farm location (village/town)")]
        public string PlaceName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Select a crop type.")]
        [Display(Name = "Crop type")]
        public string CropType { get; set; } = string.Empty;
    }
}
