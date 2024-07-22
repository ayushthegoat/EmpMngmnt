using System.ComponentModel.DataAnnotations;
using Emp.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Emp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [AgeCheck(1, 100)]
        public int Age { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DateOfBirth]
        public DateOnly Dob { get; set; }
        [Required]
        public string Address { get; set; } 
        [Required]
        [PhoneNumberCheck]
        public string PhoneNumber { get; set; }

        [Required]
        [CustomEmail]
        public string Email { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        [ValidateNever]
        public string UserId { get; set; }

        [AgeAndDateOfBirth]
        public object AgeAndDateOfBirth => new object();
    }
}
