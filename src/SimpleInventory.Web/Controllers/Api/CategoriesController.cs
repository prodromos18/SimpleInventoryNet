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

        public CategoriesController(InventoryDbContext db)
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
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return ValidationProblem("Category name is required.");
            }

            if (await _db.Categories.AnyAsync(c => c.Name == category.Name))
            {
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = "Category name already exists."
                };
                return Conflict(problem);
            }

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }

        // DELETE: /api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();

            if (category.Products.Any())
            {
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = "Cannot delete category because it has associated products."
                };
                return Conflict(problem);
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
