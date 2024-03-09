using Items.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Items.Models.Queries;
using Items.Abstractions.Queries.Handlers;
using Items.Abstractions.Queries;

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
            [FromServices] IQueryHandlerFactory<ICategoriesQueryHandler> categoriesQueryHandlerFactory,
            CancellationToken cancellationToken)
        {
            var result = await categoriesQueryHandlerFactory
                .CreateHandler()
                .ExecuteAsync(new CategoriesQuery(), cancellationToken);

            return Ok(result);
        }
    }
}
