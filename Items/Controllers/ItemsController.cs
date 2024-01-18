using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using Items.Commands;
using Items.Abstractions.Queries.Factories;
using Items.Models.Queries;
using System.Text.Json;

namespace Items.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ICommandsFactory _commandsFactory;

    public ItemsController(
        ICommandsFactory commandsFactory)
    {
        _commandsFactory = commandsFactory;
    }

    // GET: api/items/ ? page=1 & pageSize=10 & filter={filterJsonEncoded} & sort={string}
    [HttpGet(Name = "GetItemsWithPagination")]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public async Task<IActionResult> GetItemsByPage(
        [FromServices] IItemsPageQueryHandlerFactory itemsPageQueryHandlerFactory,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken,
        [FromQuery] string? filter = default,
        [FromQuery] string? sort = default)
    {
        var parsedFilter = filter != null
            ? JsonSerializer.Deserialize<FilterDto>(filter)
            : null;

        var result = await itemsPageQueryHandlerFactory
            .CreateCachedHandler()
            .ExecuteAsync(
                new ItemsPageQuery
                {
                    Page = page,
                    PageSize = pageSize,
                    Filter = parsedFilter,
                    Sort = sort
                },
                cancellationToken);

        return Ok(result);
    }

    // GET: api/items/:id
    [HttpGet("{id:guid}", Name = "GetItem")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public async Task<IActionResult> Get(
        [FromServices] IItemQueryHandlerFactory itemQueryHandlerFactory,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await itemQueryHandlerFactory
            .CreateCachedHandler()
            .ExecuteAsync(
                new ItemQuery
                {
                    ItemId = id 
                },
                cancellationToken);

        return Ok(result);
    }

    // GET: api/Items/GetItemsList ? ids={guid} & ids={guid} ...
    [HttpGet(template: "GetItemsList", Name = "GetItemList")]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public async Task<IActionResult> GetItemListAsync(
        [FromServices] IItemListQueryHandlerFactory itemListQueryHandlerFactory,
        [FromQuery] IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        var result = await itemListQueryHandlerFactory
            .CreateCachedHandler()
            .ExecuteAsync(
                new ItemListQuery { ItemsIds = ids },
                cancellationToken);

        return Ok(result);
    }

    // PUT: api/items/:id
    [HttpPut("{id:guid}", Name = "UpdateItem")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public async Task<IActionResult> UpdateItem(
        [FromRoute] Guid id,
        [FromBody, BindRequired] ItemDto itemDto,
        CancellationToken cancellationToken)
    {
        await _commandsFactory
            .CreateUpdateItemCommand(id, itemDto)
            .ExecuteAsync(cancellationToken);

        return Ok();
    }
}
