using Items.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Items.Abstractions.Queries.Factories;
using Items.Models.Queries;

namespace Items.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        public CategoriesController()
        {
        }

        // GET: api/categories/
        [HttpGet(Name = "GetAllCategories")]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorDto))]
        public async Task<IActionResult> GetAllCategoriesAsync(
            [FromServices] ICategoriesQueryHandlerFactory categoriesQueryHandlerFactory,
            CancellationToken cancellationToken)
        {
            var result = await categoriesQueryHandlerFactory
                .CreateCachedHandler()
                .ExecuteAsync(new CategoriesQuery(), cancellationToken);

            return Ok(result);
        }
    }
}
