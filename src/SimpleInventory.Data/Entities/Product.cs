using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleInventory.Data.Entities;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 3)]
    public string Sku { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    // Foreign key (CategoryId)
    public int CategoryId { get; set; }
    // Type of product is Category (nullable)
    public Category? Category { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
