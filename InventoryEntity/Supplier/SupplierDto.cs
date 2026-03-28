using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InventoryEntity.Supplier
{
    public class SupplierDto
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [StringLength(50)]
        public string? Country { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Image { get; set; }
        public IFormFile? SupplierImg { get; set; }
    }
}
