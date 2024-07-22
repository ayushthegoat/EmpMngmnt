using Emp.Models;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Emp.Areas.Identity.Pages.Account;
namespace Emp.Validation
{
    public class CustomEmailAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return $"Email field is not Valid";
        }

        private static readonly char[] AllowedCharacters =
        {
            '@', '.', '_', '%', '+', '-',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public override bool IsValid(object? value)
        {
            var email = value as string;
            if (string.IsNullOrEmpty(email)) return false;

            if (email[0] == '@' || email[0] == ' ')
            {
                return false;
            }

            if (!(email.EndsWith(".com") || email.EndsWith(".in") || email.EndsWith(".web") || email.EndsWith(".net") || email.EndsWith(".gov")))
            {
                return false;
            }

            if (email.Length > 30)
            {
                return false;
            }

            bool[] verifier = new bool[256];
            foreach (var c in AllowedCharacters)
            {
                verifier[c] = true;
            }

            for (int i = 0; i < email.Length; i++)
            {
                if (email[i] == '@' && i < 3)
                {
                    return false;
                }

                if (!verifier[email[i]])
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class PhoneNumberCheckAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return $"The Phone-Number must be 10 digits long.";
        }

        public override bool IsValid(object value)
        {
            var phoneNumber = value as string;

            if (phoneNumber == null || phoneNumber[0] == '0' || phoneNumber.IsNullOrEmpty() || phoneNumber.Length != 10 || !phoneNumber.All(char.IsDigit))
            {
                return false;
            }

            return true;
        }
    }

    public class AgeCheckAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        private readonly int _maxAge;

        public AgeCheckAttribute(int min, int max)
        {
            _minAge = min;
            _maxAge = max;
        }

        public override bool IsValid(object? value)
        {
            if (value is not int age) return false;


            return age >= _minAge && age <= _maxAge;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The Age value should be between {_minAge} & {_maxAge}";
        }
    }

    public class DateOfBirthAttribute : ValidationAttribute
    {
        public string ErrorMessage { get; set; } = "Invalid Date of Birth";

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }

        public override bool IsValid(object? value)
        {
            if (value is not DateOnly dateOfBirth) return false;

            if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now))
            {
                ErrorMessage = "The Date of Birth cannot be greater than the current day.";
                return false;
            }

            return true;
        }
    }

   
    public class AgeAndDateOfBirthAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var employee = validationContext.ObjectInstance as Employee;
            if (employee == null)
            {
                return new ValidationResult("Invalid Object Instance");
            }

            var ageOfEmployee = employee.Age;
            var yearOfBirth = employee.Dob.Year;
            var currentYear = DateOnly.FromDateTime(DateTime.Now).Year;
            var expectedAge = currentYear - yearOfBirth;

            if (expectedAge == ageOfEmployee)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Date of Birth & Age are Not Consistent");
        }
    }

    public class AgeAndDateOfBirthRegistrarAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var employee = validationContext.ObjectInstance as RegisterModel.InputModel;
            if (employee == null)
            {
                return new ValidationResult("Invalid Object Instance");
            }

            var ageOfEmployee = employee.Age;
            var yearOfBirth = employee.Dob.Year;
            var currentYear = DateOnly.FromDateTime(DateTime.Now).Year;
            var expectedAge = currentYear - yearOfBirth;

            if (expectedAge == ageOfEmployee)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Date of Birth & Age are Not Consistent");
        }
    }

    public class DateOfBirthRegistrarAttribute : ValidationAttribute
    {
        public string ErrorMessage { get; set; } = "Invalid Date of Birth";

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }

        public override bool IsValid(object? value)
        {
            if (value is not DateTime dateOfBirth) return false;

            if (dateOfBirth > DateTime.Now)
            {
                ErrorMessage = "The Date of Birth cannot be greater than the current day.";
                return false;
            }

            return true;
        }
    }
}
