using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using SimpleInventory.Data;

namespace SimpleInventory.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly InventoryDbContext _dbContext;

        public ProductsController(InventoryDbContext db)
        {
            // _db: DbContext injected via DI
            _db = db;
        }

        // GET: /Products
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                // loads the category for each product (similar to a join)
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }
    }
}