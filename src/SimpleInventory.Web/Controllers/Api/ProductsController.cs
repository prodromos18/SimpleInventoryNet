using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data;
using SimpleInventory.Data.Entities;

namespace SimpleInventory.Web.Controllers.Api
{
    [Route("api/products")]
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
        public async Task<IActionResult> Get(
            [FromQuery] string? q,
            [FromQuery] int? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p => p.Name.Contains(q) || p.Sku.Contains(q));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                items,
                total,
                page,
                pageSize
            };

            return Ok(result);
        }

        // GET: /api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _db.Products.Include(p => p.Category)
                                            .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: /api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku))
            {
                return Conflict(new { message = "SKU must be unique." });
            }

            var product = new Product
            {
                Name = dto.Name,
                Sku = dto.Sku,
                CategoryId = dto.CategoryId,
                Price = dto.Price
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        // PUT: /api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku && p.Id != id))
            {
                return Conflict(new { message = "SKU must be unique." });
            }

            product.Name = dto.Name;
            product.Sku = dto.Sku;
            product.CategoryId = dto.CategoryId;
            product.Price = dto.Price;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/products/{id}
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

    // DTO for Product create/update
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
    }
}
