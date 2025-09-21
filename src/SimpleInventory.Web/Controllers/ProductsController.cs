using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data;
using SimpleInventory.Data.Entities;

namespace SimpleInventory.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly InventoryDbContext _db;

        public ProductsController(InventoryDbContext db)
        {
            _db = db;
        }

        // GET: Products
        public async Task<IActionResult> Index(string q, int? categoryId, int page = 1)
        {
            const int pageSize = 10;

            var query = _db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(q))
                query = query.Where(p => p.Name.Contains(q) || p.Sku.Contains(q));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            query = query.OrderBy(p => p.Name);

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var total = await query.CountAsync();

            ViewBag.TotalPages = (total + pageSize - 1) / pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.Query = q;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await _db.Categories.ToListAsync();

            return View(items);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (product.Price < 0 || product.Quantity < 0)
                ModelState.AddModelError("", "Price and Quantity must be non-negative.");

            if (await _db.Products.AnyAsync(p => p.Sku == product.Sku))
                ModelState.AddModelError("Sku", "SKU must be unique.");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                return View(product);
            }

            product.UpdatedAt = System.DateTime.UtcNow;

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (product.Price < 0 || product.Quantity < 0)
                ModelState.AddModelError("", "Price and Quantity must be non-negative.");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                return View(product);
            }

            product.UpdatedAt = System.DateTime.UtcNow;

            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
