#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Items.Models.DataTransferObjects.Order
{
    public class DeliveryDetailsDto
    {
        [Required, EmailAddress]
        public string Email { get; init; }

        [Required]
        public string FirstName { get; init; }

        [Required]
        public string LastName { get; init; }

        public string? Comment { get; init; }
        public string? CompanyName { get; init; }
    }
}
