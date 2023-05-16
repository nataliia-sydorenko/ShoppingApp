using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Models
{
    public class Product
    {
        [FromHeader]
        public int Id { get; set; }
        [FromHeader]
        public string Title { get; set; } = string.Empty;
        [FromHeader]
        public double Price { get; set; }
        [FromHeader]
        public string Category { get; set; } = string.Empty;
        [FromHeader]
        public string Description { get; set; } = string.Empty;
        [FromHeader]
        public string Image { get; set; } = string.Empty;
    }
}
