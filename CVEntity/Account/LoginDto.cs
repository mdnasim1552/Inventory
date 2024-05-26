using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVEntity.Account
{
    public class LoginDto
    {     
        [StringLength(50)]
        //[Required(ErrorMessage = "sdfssd")]
        public string Email { get; set; } = null!;

        [StringLength(50)]
        public string Password { get; set; } = null!;

        public bool IsRemember { get; set; }
    }
}
