using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // needed for Swagger info
using SimpleInventory.Data;
using SimpleInventory.Data.Entities;

// Create the app builder (the host)
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Registers MVC controllers and razor views
builder.Services.AddControllersWithViews();

// Requirements say to use SQLite database
// Register InventoryDbContext with dependency injection
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite("Data Source=inventory.db"));

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SimpleInventory API",
        Version = "v1",
        Description = "API for managing products and categories"
    });
});

var app = builder.Build();

// Apply migrations and seed runtime data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    // Apply migrations
    db.Database.Migrate();

    // Seed Products if none exist
    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product { Id = 1, Name = "The brothers Karamazov", Sku = "CLA001", Price = 30, Quantity = 5, CategoryId = 1, UpdatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "Pride and Prejudice", Sku = "CLA002", Price = 20, Quantity = 10, CategoryId = 1, UpdatedAt = DateTime.UtcNow },
            new Product { Id = 3, Name = "Programming for Dummies", Sku = "NONF001", Price = 3512, Quantity = 15, CategoryId = 2, UpdatedAt = DateTime.UtcNow },
            new Product { Id = 4, Name = "Dragonball", Sku = "MAN001", Price = 2, Quantity = 25, CategoryId = 3, UpdatedAt = DateTime.UtcNow },
            new Product { Id = 5, Name = "No longer Human", Sku = "MAN002", Price = 47, Quantity = 20, CategoryId = 3, UpdatedAt = DateTime.UtcNow }
        );
        db.SaveChanges();
    }
}

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleInventory API v1");
        c.RoutePrefix = "swagger"; // UI available at /swagger
    });
}

// Configure MVC
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
