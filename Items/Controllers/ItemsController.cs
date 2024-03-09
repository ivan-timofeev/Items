using Microsoft.AspNetCore.Mvc;
using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using Items.Models.Queries;
using System.Text.Json;
using Items.Abstractions.Queries.Handlers;
using Items.Abstractions.Queries;
using Items.Models;
using Items.Abstractions.Commands;
using Items.Abstractions.Commands.Handlers;
using Items.Models.Commands;

namespace Items.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    // GET: api/items/ ? page=1 & pageSize=10 & filter={filterJsonEncoded} & sort={string}
    [HttpGet(Name = "GetItemsWithPagination")]
    public async Task<PaginatedResult<ItemDto>> GetItemsByPage(
        [FromServices] IQueryHandlerFactory<IItemsPageQueryHandler> handlerFactory,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken,
        [FromQuery] string? filter = default,
        [FromQuery] string? sort = default)
    {
        var parsedFilter = filter != null
            ? JsonSerializer.Deserialize<FilterDto>(filter)
            : null;

        var result = await handlerFactory
            .CreateHandler()
            .ExecuteAsync(
                new ItemsPageQuery
                {
                    Page = page,
                    PageSize = pageSize,
                    Filter = parsedFilter,
                    Sort = sort
                },
                cancellationToken);

        return result;
    }

    // GET: api/items/:id
    [HttpGet("{id:guid}", Name = "GetItem")]
    public async Task<ItemDto> Get(
        [FromServices] IQueryHandlerFactory<IItemQueryHandler> handlerFactory,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await handlerFactory
            .CreateHandler()
            .ExecuteAsync(
                new ItemQuery
                {
                    ItemId = id 
                },
                cancellationToken);

        return result;
    }

    // GET: api/Items/GetItemsList ? ids={guid} & ids={guid} ...
    [HttpGet(template: "GetItemsList", Name = "GetItemList")]
    public async Task<IEnumerable<ItemDto>> GetItemListAsync(
        [FromServices] IQueryHandlerFactory<IItemListQueryHandler> handlerFactory,
        [FromQuery] IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        var result = await handlerFactory
            .CreateHandler()
            .ExecuteAsync(
                new ItemListQuery { ItemsIds = ids },
                cancellationToken);

        return result;
    }

    // PUT: api/items/:id
    [HttpPut("{id:guid}", Name = "UpdateItem")]
    public async Task UpdateItem(
        [FromServices] ICommandHandlerFactory<IUpdateItemCommandHandler> handlerFactory,
        [FromRoute] Guid id,
        [FromBody] ItemDto itemDto,
        CancellationToken cancellationToken)
    {
        await handlerFactory
            .CreateHandler()
            .ExecuteAsync(
                new UpdateItemCommand
                {
                    ItemId = id,
                    ItemDto = itemDto
                },
                cancellationToken);
    }
}
