using Items.Commands;
using Items.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Items.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly ICommandsFactory _commandsFactory;

        public HealthCheckController(ICommandsFactory commandsFactory)
        {
            _commandsFactory = commandsFactory;
        }

        // GET: api/HealthCheck
        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok(new { Status = "OK" });
        }

        // GET: api/HealthCheck
        [Authorize]
        [HttpGet("Authorization")]
        public IActionResult AuthorizationHealthCheck()
        {
            return Ok(new { Status = "OK" });
        }

        // GET: api/HealthCheck/Database
        [HttpGet("Database")]
        public async Task<IActionResult> DatabaseHealthCheck(CancellationToken cancellationToken)
        {
            try
            {
                await _commandsFactory
                    .CreateEnsureIsDatabaseAliveCommand()
                    .ExecuteAsync(cancellationToken);

                return Ok(new { Status = "OK" });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Status = "ERROR", Details = ex.Message });
            }
        }
    }
}
