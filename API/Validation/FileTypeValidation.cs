using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace API.Validation
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] _validTypes;

        public FileTypeValidation(string[] validTypes)
        {
            _validTypes = validTypes;
        }

        public FileTypeValidation(FileTypeGroup fileTypeGroup)
        {
            if (fileTypeGroup == FileTypeGroup.File)
            {
                _validTypes = new string[] {"application/pdf"};
            }
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

            if (!_validTypes.Contains((formFile.ContentType)))
            {
                return new ValidationResult($"El tipo de archivo debe ser: {_validTypes[0]}");
            }


            return ValidationResult.Success;
        }
    }
}