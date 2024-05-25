using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVEntity.Account
{
    public class RegisterDto
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Email { get; set; } = null!;

        [StringLength(50)]
        public string Password { get; set; } = null!;
        [StringLength(50)]
        public string ConfirmPassword { get; set; } = null!;
        [Required(ErrorMessage = "Please select a gender.")]
        public string Gender { get; set; } = null!;

        [Column(TypeName = "smalldatetime")]
        public DateTime Birthday { get; set; }
    }
}
