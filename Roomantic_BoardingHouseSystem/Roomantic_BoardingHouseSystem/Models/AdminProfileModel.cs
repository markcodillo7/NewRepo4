using System.ComponentModel.DataAnnotations;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class AdminProfileModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;
    }
}
