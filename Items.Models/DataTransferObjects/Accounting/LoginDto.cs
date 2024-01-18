using System.ComponentModel.DataAnnotations;

namespace Items.Models.DataTransferObjects.Accounting
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public required string Email { get; init; }

        [Required, MinLength(4)]
        public required string Password { get; init; }
    }
}
