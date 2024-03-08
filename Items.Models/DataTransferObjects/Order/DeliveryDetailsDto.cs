using System.ComponentModel.DataAnnotations;

namespace Items.Models.DataTransferObjects.Order
{
    public class DeliveryDetailsDto
    {
        [EmailAddress]
        public required string Email { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? Comment { get; init; }
        public string? CompanyName { get; init; }
    }
}
