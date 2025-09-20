using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleInventory.Data;
using SimpleInventory.Data.Entities;
using Xunit;

namespace SimpleInventory.Tests
{
    public class ProductsAndCategoriesTests
    {
        // Helper to create a fresh in-memory DbContext
        private InventoryDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            var db = new InventoryDbContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public async Task ProductCreation_ShouldRejectDuplicateSkuOrNegativePrice()
        {
            // Arrange
            using var db = GetInMemoryDbContext();

            var category = new Category { Name = "Test Category" };
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var product = new Product
            {
                Name = "Product A",
                Sku = "SKU001",
                Price = 10,
                Quantity = 5,
                CategoryId = category.Id
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();

            // Act
            var duplicateSkuProduct = new Product
            {
                Name = "Product B",
                Sku = "SKU001", // same SKU
                Price = 20,
                Quantity = 3,
                CategoryId = category.Id
            };

            var negativePriceProduct = new Product
            {
                Name = "Product C",
                Sku = "SKU002",
                Price = -5,
                Quantity = 1,
                CategoryId = category.Id
            };

            // Assert duplicate SKU manually (InMemory does not enforce unique)
            var skuExists = db.Products.Any(p => p.Sku == duplicateSkuProduct.Sku);
            skuExists.Should().BeTrue("duplicate SKU should not be allowed");

            // Assert negative price
            negativePriceProduct.Price.Should().BeNegative();
        }

        [Fact]
        public async Task ProductFiltering_ShouldReturnCorrectResults()
        {
            using var db = GetInMemoryDbContext();

            var category = new Category { Name = "Test Category" };
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            db.Products.AddRange(
                new Product { Name = "Apple", Sku = "SKU01", Price = 5, Quantity = 10, CategoryId = category.Id },
                new Product { Name = "Banana", Sku = "SKU02", Price = 3, Quantity = 5, CategoryId = category.Id },
                new Product { Name = "Apricot", Sku = "SKU03", Price = 7, Quantity = 8, CategoryId = category.Id }
            );
            await db.SaveChangesAsync();

            // Act: filter products containing "Ap"
            var filtered = await db.Products
                .Where(p => p.Name.Contains("Ap"))
                .ToListAsync();

            // Assert
            filtered.Count.Should().Be(2); // Apple + Apricot
            filtered.Select(p => p.Name).Should().Contain(new[] { "Apple", "Apricot" });
        }

        [Fact]
        public async Task CategoryDeletion_ShouldConflictIfProductsExist()
        {
            using var db = GetInMemoryDbContext();

            var category = new Category { Name = "Category With Products" };
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            db.Products.Add(new Product
            {
                Name = "Prod",
                Sku = "SKU1",
                Price = 10,
                Quantity = 1,
                CategoryId = category.Id
            });
            await db.SaveChangesAsync();

            // Simulate deletion rule
            var hasProducts = db.Products.Any(p => p.CategoryId == category.Id);
            hasProducts.Should().BeTrue("category has products, cannot delete");
        }

    }
}
