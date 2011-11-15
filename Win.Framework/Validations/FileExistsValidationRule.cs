#region Using

using System.Globalization;
using System.IO;
using System.Windows.Controls;

#endregion

namespace Willcraftia.Win.Framework.Validations
{
    public sealed class FileExistsValidationRule : ValidationRule
    {
        const string DefaultErrorMessageFormat = "File '{0}' does not exist.";

        string errorMessageFormat = DefaultErrorMessageFormat;
        public string ErrorMessageFormat
        {
            get { return errorMessageFormat; }
            set { errorMessageFormat = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (File.Exists(input))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, string.Format(errorMessageFormat, input));
            }
        }
    }
}
