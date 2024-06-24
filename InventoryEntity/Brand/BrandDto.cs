using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace InventoryEntity.Brand
{
    public class BrandDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public IFormFile? BrandImg { get; set; }
    }
}
