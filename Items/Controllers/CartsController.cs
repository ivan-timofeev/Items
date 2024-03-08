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
        [HttpPost]
        public async Task<IActionResult> ApplyPromocode(
            [FromServices] IApplyPromocodeQueryHandlerFactory applyPromocodeQueryHandlerFactory,
            [FromBody] ApplyPromocodeQuery applyPromocodeCommandDto,
            CancellationToken cancellationToken)
        {
            var result = await applyPromocodeQueryHandlerFactory
                   .CreateHandler()
                   .ExecuteAsync(applyPromocodeCommandDto, cancellationToken);

            return Ok(result);
        }
    }
}
