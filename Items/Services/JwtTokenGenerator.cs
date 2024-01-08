using Items.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Items.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                         ?? throw new InvalidOperationException("Jwt:Key must be specified.");
            var jwtIssuer = "TIMOFEEV-IDENTITY";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                jwtIssuer,
                audience: null,
                claims: CreateClaims(),
                expires: DateTime.Now.AddDays(365),
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;

            Claim[] CreateClaims()
            {
                return new[]
                {
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName),
                    new Claim("email", user.Email),
                    new Claim("role", "user")
                };
            }
        }
    }
}
