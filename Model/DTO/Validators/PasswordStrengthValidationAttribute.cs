using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LiveWaitlistServer.Model.DTO.Validators
{
    public class PasswordStrengthValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (!(value is string password))
                return false;

            if (!password.Any(c => char.IsLetter(c)))
            {
                ErrorMessage = "Password must contain at least 1 letter.";
                return false;
            }

            if (!password.Any(c => char.IsDigit(c)))
            {
                ErrorMessage = "Password must contain at least 1 digit.";
                return false;
            }

            return true;
        }
    }
}