
using System.ComponentModel.DataAnnotations;

namespace Emp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }
        [Required]
        public DateOnly Dob { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
    }
}
