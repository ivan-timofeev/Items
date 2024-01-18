using Items.Models;

namespace Items.Abstractions.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
