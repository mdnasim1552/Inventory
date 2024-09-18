using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace InventoryEntity.Account
{
    public class UserDto
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]      
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Confirm Password is required")]      
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }=null!;

        //[Required(ErrorMessage = "Password is required")]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "Confirm Password is required")]
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        //public string ConfirmPassword { get; set; }

        [StringLength(1)]     
        public string? Gender { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? Birthday { get; set; }

        public int? RoleId { get; set; }

        [StringLength(500)]
        public string? Image { get; set; }
        public IFormFile? UserImg { get; set; }
        [StringLength(15)]
        public string? Mobile { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? CreatedOn { get; set; }
    }
}
