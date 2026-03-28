using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text;

namespace InventoryEntity.Store
{
    public class StoreDto
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Code { get; set; } = null!;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
    }
}
