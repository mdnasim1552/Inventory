using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace InventoryEntity.SubCategory
{
    public class SubCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        public string? Description { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? SubCategoryImg { get; set; }
    }
}
