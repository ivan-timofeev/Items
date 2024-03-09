using Items.Abstractions.Queries;
using Items.Abstractions.Queries.Handlers;
using Items.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Items.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        [HttpPost(template: "ApplyPromocode")]
        public async Task<IActionResult> ApplyPromocodeAsync(
            [FromServices] IQueryHandlerFactory<IApplyPromocodeQueryHandler> handlerFactory,
            [FromBody] ApplyPromocodeQuery applyPromocodeQuery,
            CancellationToken cancellationToken)
        {
            var result = await handlerFactory
                   .CreateHandler()
                   .ExecuteAsync(applyPromocodeQuery, cancellationToken);

            return Ok(result);
        }
    }
}
