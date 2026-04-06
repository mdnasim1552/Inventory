using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventoryEntity.Product
{
    public class ProductViewDto
    {
        public int Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public int UnitId { get; set; }
        public string? UnitShortName { get; set; }
        public string? Role { get; set; }
        [StringLength(50)]
        public string? Sku { get; set; }
        public string? Description { get; set; }
        [StringLength(500)]
        public string? Image { get; set; }
        public IFormFile? ProductImg { get; set; }
        public int CreatedBy { get; set; }
    }
}
