using Items.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Items.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
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
            return Ok();
            /*
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
            */
        }
    }
}
