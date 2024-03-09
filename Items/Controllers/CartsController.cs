using Items.Abstractions.Commands.Factories;
using Items.Abstractions.Queries.Factories;
using Items.Helpers;
using Items.Models.DataTransferObjects.Order;
using Items.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Items.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        [HttpPost(template: "ApplyPromocode")]
        public async Task<IActionResult> ApplyPromocodeAsync(
            [FromServices] IApplyPromocodeQueryHandlerFactory applyPromocodeQueryHandlerFactory,
            [FromBody] ApplyPromocodeQuery applyPromocodeQuery,
            CancellationToken cancellationToken)
        {
            var result = await applyPromocodeQueryHandlerFactory
                   .CreateHandler()
                   .ExecuteAsync(applyPromocodeQuery, cancellationToken);

            return Ok(result);
        }
    }
}
