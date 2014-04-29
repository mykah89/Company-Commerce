using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Validation.CustomValidation
{
    public class PasswordValidator
    {
        public List<ValidationError> Validate(String password)
        {
            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            List<ValidationError> result = new List<ValidationError>();

            if (password.Length < 8)
            {
                result.Add(new ValidationError("Password", "Password length must be more than 8 characters.", password));
            }

            return result;
        }
    }
}
