#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

#endregion

namespace Willcraftia.Win.Framework.Validations
{
    public class DirectoryNameValidationRule : ValidationRule
    {
        const string DefaultErrorMessageFormat = "Must not contain '{0}'.";

        string errorMessageFormat = DefaultErrorMessageFormat;
        public string ErrorMessageFormat
        {
            get { return errorMessageFormat; }
            set { errorMessageFormat = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            var errorIndex = input.IndexOfAny(Path.GetInvalidPathChars());
            if (0 <= errorIndex)
            {
                var errorChar = input[errorIndex];

                return new ValidationResult(false, CreateErrorContent(input, errorIndex, errorChar));
            }

            return ValidationResult.ValidResult;
        }

        protected virtual object CreateErrorContent(string value, int errorIndex, char errorChar)
        {
            return string.Format(errorMessageFormat, errorChar);
        }
    }
}
