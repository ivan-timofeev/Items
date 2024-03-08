using Items.Abstractions.Commands.Factories;
using Items.Abstractions.Queries.Factories;
using Items.Models.DataTransferObjects.Order;
using Items.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Items.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromServices] ICreateOrderCommandHandlerFactory createOrderCommandHandlerFactory,
            [FromBody] CreateOrderBase createOrderDto,
            CancellationToken cancellationToken)
        {
            await createOrderCommandHandlerFactory
                   .CreateHandler()
                   .ExecuteAsync(createOrderDto, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("Search")]
        public async Task<IActionResult> GetOrdersAsync(
            [FromServices] IOrdersQueryHanlderFactory ordersQueryHanlderFactory,
            [FromBody] OrdersQueryBase ordersQuery,
            CancellationToken cancellationToken)
        {
            var result = await ordersQueryHanlderFactory
                .CreateHandler()
                .ExecuteAsync(ordersQuery, cancellationToken);

            return Ok(result);
        }
    }
}
