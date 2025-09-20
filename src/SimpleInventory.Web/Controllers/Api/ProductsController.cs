using Microsoft.AspNetCore.Mvc; // Classes: ControllerBase, IActionResult, att: Route, ApiController, HttpGet
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data;

namespace SimpleInventory.Web.Controllers.Api
{
    [Route("api/products")]
    // Marks this class as an API controller
    // Enables automatic model validation and JSON responses
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly InventoryDbContext _db;

        public ProductsController(InventoryDbContext db)
        {
            _db = db;
        }

        // GET: /api/products
        [HttpGet]
        // async to allow non blocking calls
        public async Task<IActionResult> Get()
        {
            // fetches all products from the database, including their associated categories
            var products = await _db.Products.Include(p => p.Category).ToListAsync();
            // returns the products as a JSON response with HTTP 200 OK status
            return Ok(products);
        }
    }
}
