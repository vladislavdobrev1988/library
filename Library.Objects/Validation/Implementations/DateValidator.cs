using System;
using System.Globalization;
using Library.Objects.Helpers.Constants;
using Library.Objects.Validation.Interfaces;

namespace Library.Objects.Validation.Implementations
{
    public class DateValidator : IDateValidator
    {
        private static class ErrorMessage
        {
            public const string REQUIRED = "Date is required";
            public const string INVALID_FORMAT = "\"{0}\" is invalid date. Expected format is: {1}";
        }

        public string Validate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return ErrorMessage.REQUIRED;
            }

            if (!DateTime.TryParseExact(date, DateFormat.ISO_8601, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                return string.Format(ErrorMessage.INVALID_FORMAT, date, DateFormat.ISO_8601);
            }

            return null;
        }
    }
}
