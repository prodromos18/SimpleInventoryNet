using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleInventory.Data.Entities;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 3)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int Quantity { get; set; }

    // Foreign key (CategoryId)
    public int CategoryId { get; set; }
    // Type of product is Category (nullable)
    public Category? Category { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
