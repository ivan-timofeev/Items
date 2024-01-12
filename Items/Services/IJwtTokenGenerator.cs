using Items.Models;

namespace Items.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
