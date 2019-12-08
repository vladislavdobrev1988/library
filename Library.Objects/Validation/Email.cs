using System.ComponentModel.DataAnnotations;

namespace Library.Objects.Validation
{
    public static class Email
    {
        private static readonly EmailAddressAttribute _validator;

        private static class ErrorMessage
        {
            public const string REQUIRED = "Email is required";
            public const string INVALID_EMAIL_FORMAT = "\"{0}\" is invalid email address";
        }

        static Email()
        {
            _validator = new EmailAddressAttribute();
        }

        public static string Validate(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ErrorMessage.REQUIRED;
            }

            if (!_validator.IsValid(email))
            {
                return string.Format(ErrorMessage.INVALID_EMAIL_FORMAT, email);
            }

            return null;
        }
    }
}
