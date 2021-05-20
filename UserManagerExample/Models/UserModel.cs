using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagerExample.Models
{
    public class UserModel : IdentityUser
    {
        [Required]
        public string Fname { get; set; }

        [Required]
        public string Sname { get; set; }

        [Required]
        [MaxLength(11)]
        public string Tel { get; set; }
    }
}
