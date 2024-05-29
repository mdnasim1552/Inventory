using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryEntity.Account
{
    public class RegisterDto
    {
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Email { get; set; } = null!;

        [StringLength(50)]
        public string Password { get; set; } = null!;          
    }
}
