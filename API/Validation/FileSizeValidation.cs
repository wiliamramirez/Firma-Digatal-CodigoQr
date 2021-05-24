using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace API.Validation
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int _maximumSizeInMegabytes;

        public FileSizeValidation(int maximumSizeInMegabytes)
        {
            _maximumSizeInMegabytes = maximumSizeInMegabytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (formFile.Length > _maximumSizeInMegabytes * 1024 * 1024)
            {
                return new ValidationResult($"El peso del documento no debe ser mayor a {_maximumSizeInMegabytes}mb");
            }
            
            return ValidationResult.Success;
        }
    }
}