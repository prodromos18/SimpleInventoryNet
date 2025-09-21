using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data;
using SimpleInventory.Data.Entities;

namespace SimpleInventory.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly InventoryDbContext _db;

        public CategoriesController(InventoryDbContext db)
        {
            _db = db;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _db.Categories.ToListAsync();
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create() => View();

        // POST: Categories/Create
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (await _db.Categories.AnyAsync(c => c.Name == category.Name))
                ModelState.AddModelError("Name", "Category name must be unique.");

            if (!ModelState.IsValid) return View(category);

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
