using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Emp.Models
{
    public class ApplicationUser : IdentityUser
    {
      
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime Dob { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

      
    }
}
