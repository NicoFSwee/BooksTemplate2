using System.ComponentModel.DataAnnotations;

namespace Books.Core.Validations
{
    public class IsbnValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string isbn = (string)value;

            if(isbn == null)
            {
                return new ValidationResult("ISBN ist ungültig!");
            }

            int checkSum = 0;

            for (int i = 0; i < isbn.Length; i++)
            {
                if (!char.IsDigit(isbn[i]) && i != 9 || isbn.Length != 10)
                {
                    return new ValidationResult("ISBN ist ungültig!");
                }
                else if (!char.IsDigit(isbn[i]) && i == 9)
                {
                    if (isbn[i] != 'x' && isbn[i] != 'X')
                    {
                        return new ValidationResult("ISBN ist ungültig!");
                    }
                }

                if (char.IsDigit(isbn[i]))
                {
                    int number = int.Parse(isbn[i].ToString());
                    checkSum += number * (i + 1);
                }
                else if (isbn[i] == 'x' || isbn[i] == 'X')
                {
                    checkSum += 10 * (i + 1);
                }
            }

            if (checkSum % 11 == 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("ISBN ist ungültig!");
            }

            
        }
    }
}
