using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data;
using SimpleInventory.Data.Entities;

namespace SimpleInventory.Web.Controllers.Api
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly InventoryDbContext _db;

        public CategoriesController(InventoryDbContext db)
        {
            _db = db;
        }

        // GET: /api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            return Ok(await _db.Categories.ToListAsync());
        }

        // GET: /api/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // POST: /api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (await _db.Categories.AnyAsync(c => c.Name == category.Name))
            {
                return Conflict(new { message = "Category name already exists." });
            }

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
        }
    }
}
