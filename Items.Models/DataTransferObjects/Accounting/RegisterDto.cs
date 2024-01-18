using System.ComponentModel.DataAnnotations;

namespace Items.Models.DataTransferObjects.Accounting
{
    public sealed class RegisterDto
    {
        [Required, EmailAddress]
        public required string Email { get; init; }

        [Required]
        public required string FirstName { get; init; }

        [Required]
        public required string LastName { get; init; }

        [Required, MinLength(4)]
        public required string Password { get; init; }

        public string? CompanyName { get; init; }
    }
}
