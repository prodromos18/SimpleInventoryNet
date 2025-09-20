using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleInventory.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public List<Product> Products { get; set; } = new();
    }
}
