// DbContext: Central class that manages database connections and entity sets.
using System;
// EF Core ORM
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data.Entities;

namespace SimpleInventory.Data
{
    // DbContext class that inherits from EF Core's DbContext.
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        // DbSet for Products
        public DbSet<Product> Products { get; set; } = default!;

        // DbSet for Categories
        public DbSet<Category> Categories { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint on SKU
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Sku)
            .IsUnique();

        // Relationship: Category has many Products
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
                
            // Seed Categories only, since only static data can be seeded here
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Classics" },
                new Category { Id = 2, Name = "Non-Fiction" },
                new Category { Id = 3, Name = "Manga" }
            );
        }
    }
}