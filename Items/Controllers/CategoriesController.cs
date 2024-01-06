using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Items.Data;
using Microsoft.EntityFrameworkCore;

namespace Items.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;

        public CategoriesController(IDbContextFactory<ItemsDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // GET: api/categories/
        [HttpGet(Name = "GetAllCategories")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorDto))]
        //[ResponseCache(VaryByHeader = "UserAgent", Duration = 300)]
        public async Task<IActionResult> Index()
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var categories = await dbContext
                .ItemsCategory
                .Include(ic => ic.Items)
                .Select(ic =>
                    new CategoryDto
                    {
                        DisplayName = ic.DisplayName,
                        ProductsInCategory = ic.Items.Count
                    })
                .OrderByDescending(ic => ic.ProductsInCategory)
                .ToArrayAsync();

            return Ok(categories);
        }
    }
}
