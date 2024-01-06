using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Items.Data;
using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using System.Text.Json;

namespace Items.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ILogger<ItemsController> _logger;
    private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;

    public ItemsController(
        ILogger<ItemsController> logger,
        IDbContextFactory<ItemsDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    // GET: api/items/ ? page=1 & pageSize=10
    [HttpGet(Name = "GetItemsWithPagination")]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    [ResponseCache(VaryByQueryKeys = new[] { "page", "pageSize" }, Duration = 300)]
    public IActionResult GetItemsByPage(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? filter = default,
        [FromQuery] string? sort = default)
    {
        // TODO вынести это все в сервисы, нужно избавиться от этого лютого дерьма

        var parsedFilter = filter != null
            ? JsonSerializer.Deserialize<FilterDto>(filter)
            : null;

        using var dbContext = _dbContextFactory.CreateDbContext();
        var itemsQuery = dbContext
            .Items
            .Include(i => i.Categories)
            .Where(i => parsedFilter != null || i.Categories.Any(c => parsedFilter.SelectedCategories.Contains(c.DisplayName)))
            .Select(i => 
                new ItemDto
                { 
                    Id = i.Id,
                    AvailableQuantity = i.AvailableQuantity,
                    Description = i.Description,
                    DisplayName = i.DisplayName,
                    ImageUrl = i.ImageUrl,
                    OverallRating = i.OverallRating,
                    Price = i.Price,
                    Categories = i.Categories
                        .Select(c => c.DisplayName)
                        .ToArray()
                });

        if (parsedFilter != default)
        {
            if (parsedFilter.SelectedCategories.Any())
                itemsQuery = itemsQuery.Where(i => i.Categories.Any(c => parsedFilter.SelectedCategories.Contains(c)));

            if (parsedFilter.SelectedPriceRange.From != null)
                itemsQuery = itemsQuery.Where(i => i.Price >= parsedFilter.SelectedPriceRange.From);

            if (parsedFilter.SelectedPriceRange.To != null)
                itemsQuery = itemsQuery.Where(i => i.Price <= parsedFilter.SelectedPriceRange.To);
        }

        if (sort == "price-desc")
        {
            itemsQuery = itemsQuery.OrderByDescending(i => i.Price);
        }
        else if (sort == "price-asc")
        {
            itemsQuery = itemsQuery.OrderBy(i => i.Price);
        }

        var items = itemsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        var count = itemsQuery.Count();

        var result = new PaginatedResult<ItemDto>()
        {
            TotalElementsCount = count,
            PageElementsCount = items.Count(),
            Elements = items,
            CurrentPageNumber = page,
            MaxPageNumber = (int)Math.Ceiling((decimal)(count) / pageSize)
        };

        return Ok(result);
    }

    // GET: api/items/:id
    [HttpGet("{id:guid}", Name = "GetItem")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    [ResponseCache(VaryByQueryKeys = new[] { "id" }, Duration = 300)]
    public IActionResult Get(Guid id)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var item = dbContext
            .Items
            .Where(i => i.Id == id)
            .Select(i =>
                new ItemDto
                {
                    Id = i.Id,
                    AvailableQuantity = i.AvailableQuantity,
                    Description = i.Description,
                    DisplayName = i.DisplayName,
                    ImageUrl = i.ImageUrl,
                    OverallRating = i.OverallRating,
                    Price = i.Price,
                    Categories = i.Categories
                        .Select(c => c.DisplayName)
                        .ToArray()
                })
            .Single();

        return Ok(item);
    }

    // POST: api/items/
    [HttpPost(Name = "CreateItem")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorDto))]
    public IActionResult CreateNew([FromBody, BindRequired] CreateItemDto createItemDto)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var newItem = new Item
        {
            DisplayName = createItemDto.DisplayName,
            AvailableQuantity = createItemDto.Quantity,
            Price = createItemDto.Price,
            Categories = createItemDto
                .Categories
                .Select(c => new ItemCategory { DisplayName = c })
                .ToList(),
            Description = createItemDto.Description,
            ImageUrl = createItemDto.ImageUrl,
            OverallRating = createItemDto.OverallRating
        };
        dbContext.Items.Add(newItem);
        dbContext.SaveChanges();

        return Ok(newItem.Id);
    }
}
