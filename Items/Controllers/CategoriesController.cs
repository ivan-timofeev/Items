using Items.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Items.Queries;

namespace Items.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IQueriesFactory _queriesFactory;

        public CategoriesController(IQueriesFactory queriesFactory)
        {
            _queriesFactory = queriesFactory;
        }

        // GET: api/categories/
        [HttpGet(Name = "GetAllCategories")]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorDto))]
        public async Task<IActionResult> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            var result = await _queriesFactory
                .CreateGetAllCategoriesQuery()
                .ExecuteAsync(cancellationToken);

            return Ok(result);
        }
    }
}
