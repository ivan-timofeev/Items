using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Items.Models.DataTransferObjects.Accounting;
using Items.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using Items.Models;
using System.Threading;
using Items.Services;

namespace Items.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountingController : ControllerBase
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AccountingController(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _dbContextFactory = dbContextFactory;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        // POST: api/login
        [HttpPost(template: "Login", Name = "Login")]
        public async Task<IActionResult> Login(
            [FromBody, BindRequired] LoginDto loginDto,
            CancellationToken cancellationToken)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var user = dbContext
                .Users
                .FirstOrDefault(u => string.Equals(
                    u.Email,
                    loginDto.Email));

            if (user == default)
            {
                return BadRequest("User with provided email not found.");
            }

            var passwordHash = GetSha256(loginDto.Password);

            if (!string.Equals(
                user.PasswordHash,
                passwordHash,
                StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized("Wrong password.");
            }

            var result = _jwtTokenGenerator.GenerateToken(user);
            return Ok(result);
        }

        // POST: api/register
        [HttpPost(template: "Register", Name = "Register")]
        public async Task<IActionResult> RegisterAsync(
            [FromBody, BindRequired] RegisterDto registerDto,
            CancellationToken cancellationToken)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var user = dbContext
                .Users
                .FirstOrDefault(u => string.Equals(
                    u.Email,
                    registerDto.Email));

            if (user != default)
            {
                return BadRequest("User with provided email already created.");
            }

            var newUser = new User
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                CompanyName = registerDto.CompanyName,
                PasswordHash = GetSha256(registerDto.Password)
            };

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        private static string GetSha256(string value)
        {
            var stringBuilder = new StringBuilder();
            var encoding = Encoding.UTF8;
            var result = SHA256.HashData(encoding.GetBytes(value));

            foreach (byte b in result)
                stringBuilder.Append(b.ToString("x2"));

            return stringBuilder.ToString();
        }
    }
}
