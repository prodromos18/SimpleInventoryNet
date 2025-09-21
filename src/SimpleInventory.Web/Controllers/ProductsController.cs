using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data;
using SimpleInventory.Data.Entities;

namespace SimpleInventory.Web.Api
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<object>> Get(
            string? q = null,
            int? categoryId = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.Name.Contains(q) || p.Sku.Contains(q));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                items,
                total,
                page,
                pageSize
            });
        }

        // GET: /api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: /api/products
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] ProductDto dto)
        {
            // Validation
            if (dto.Price < 0 || dto.Quantity < 0)
                return ValidationProblem("Price and Quantity must be non-negative.");

            if (dto.Sku.Length < 3 || dto.Sku.Length > 32)
                return ValidationProblem("SKU must be 3-32 characters long.");

            if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku))
                return Conflict(new { message = "SKU must be unique." });

            var category = await _db.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest(new { message = "Category does not exist." });

            var product = new Product
            {
                Name = dto.Name,
                Sku = dto.Sku,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: /api/products/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Update(int id, [FromBody] ProductDto dto)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (dto.Price < 0 || dto.Quantity < 0)
                return ValidationProblem("Price and Quantity must be non-negative.");

            if (dto.Sku.Length < 3 || dto.Sku.Length > 32)
                return ValidationProblem("SKU must be 3-32 characters long.");

            if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku && p.Id != id))
                return Conflict(new { message = "SKU must be unique." });

            var category = await _db.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest(new { message = "Category does not exist." });

            product.Name = dto.Name;
            product.Sku = dto.Sku;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            return Ok(product);
        }

        // DELETE: /api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    public class ProductDto
    {
        public string Name { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
    }
}
