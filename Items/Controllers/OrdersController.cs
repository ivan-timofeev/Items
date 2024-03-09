using Items.Abstractions.Commands;
using Items.Abstractions.Commands.Handlers;
using Items.Abstractions.Queries;
using Items.Abstractions.Queries.Handlers;
using Items.Models.Commands;
using Items.Models.Exceptions;
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
            [FromServices] ICommandHandlerFactory<ICreateOrderCommandHandler> createOrderCommandHandlerFactory,
            [FromBody] CreateOrderCommandBase createOrderCommand,
            CancellationToken cancellationToken)
        {
            if (createOrderCommand is CreateOrderFromUserCommand createOrderFromUserCommand)
            {
                var userId = User
                    .Claims
                    .Where(c => c.Type == "userId")
                    .SingleOrDefault()
                    ?.Value
                    ?? throw new InvalidOperationException("Jwt must contain a 'userId' claim.");

                if (createOrderFromUserCommand.UserId != Guid.Parse(userId))
                {
                    throw new BusinessException(
                        ListOfBusinessErrors.IncorrectUserSpecified,
                        new()
                        {
                            { "JwtUserId", userId },
                            { "CommandUserId", createOrderFromUserCommand.UserId.ToString() }
                        });
                }
            }

            await createOrderCommandHandlerFactory
                   .CreateHandler()
                   .ExecuteAsync(createOrderCommand, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("Search")]
        public async Task<IActionResult> GetOrdersAsync(
            [FromServices] IQueryHandlerFactory<IOrdersQueryHandler> hanlderFactory,
            [FromBody] OrdersQueryBase ordersQuery,
            CancellationToken cancellationToken)
        {
            var result = await hanlderFactory
                .CreateHandler()
                .ExecuteAsync(ordersQuery, cancellationToken);

            return Ok(result);
        }
    }
}
