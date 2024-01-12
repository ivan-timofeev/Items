using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Items.Data;
using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using System.Text.Json;
using Items.Queries;

namespace Items.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IQueriesFactory _queriesFactory;

    public ItemsController(IQueriesFactory queriesFactory)
    {
        _queriesFactory = queriesFactory;
    }

    // GET: api/items/ ? page=1 & pageSize=10
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
        // TODO move to services, govnocode

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

    // POST: api/items/
    [HttpPost(Name = "CreateItem")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public IActionResult CreateNew([FromBody, BindRequired] CreateItemDto createItemDto)
    {

        return Ok();
    }
}

