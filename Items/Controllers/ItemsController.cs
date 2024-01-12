using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using System.Text.Json;
using Items.Queries;
using Items.Commands;

namespace Items.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IQueriesFactory _queriesFactory;
    private readonly ICommandsFactory _commandsFactory;

    public ItemsController(
        IQueriesFactory queriesFactory,
        ICommandsFactory commandsFactory)
    {
        _queriesFactory = queriesFactory;
        _commandsFactory = commandsFactory;
    }

    // GET: api/items/ ? page=1 & pageSize=10 & filter={filterJsonEncoded} & sort={string}
    [HttpGet(Name = "GetItemsWithPagination")]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public async Task<IActionResult> GetItemsByPage(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken,
        [FromQuery] string? filter = default,
        [FromQuery] string? sort = default)
    {
        var parsedFilter = filter != null
            ? JsonSerializer.Deserialize<FilterDto>(filter)
            : null;

        var result = await _queriesFactory
            .CreateGetItemsPageQuery(page, pageSize, parsedFilter, sort)
            .ExecuteAsync(cancellationToken);

        return Ok(result);
    }

    // GET: api/items/:id
    [HttpGet("{id:guid}", Name = "GetItem")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _queriesFactory
            .CreateGetItemQuery(id)
            .ExecuteAsync(cancellationToken);

        return Ok(result);
    }

    // GET: api/Items/GetItemsList ? ids={guid} & ids={guid} ...
    [HttpGet(template: "GetItemsList", Name = "GetItemsList")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public async Task<IActionResult> GetItemsListAsync(
        [FromQuery] IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        var result = await _queriesFactory
            .CreateGetItemsListQuery(ids)
            .ExecuteAsync(cancellationToken);

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
