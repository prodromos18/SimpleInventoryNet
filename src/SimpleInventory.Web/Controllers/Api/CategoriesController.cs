using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data;
using SimpleInventory.Data.Entities;

namespace SimpleInventory.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly InventoryDbContext _db;

        public CategoriesController(InventoryDbContext db) // DI inject
        {
            _db = db;
        }

        // GET: /api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _db.Categories.ToListAsync();
        }

        // POST: /api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            // Validate uniqueness
            if (await _db.Categories.AnyAsync(c => c.Name == category.Name))
            {
                return Conflict(new { message = "Category name already exists." });
            }

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }
    }
}
