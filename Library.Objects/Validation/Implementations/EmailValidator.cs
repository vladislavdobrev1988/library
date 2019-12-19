using System.ComponentModel.DataAnnotations;
using Library.Objects.Helpers.Constants;
using Library.Objects.Validation.Interfaces;

namespace Library.Objects.Validation.Implementations
{
    public class EmailValidator : IEmailValidator
    {
        private readonly EmailAddressAttribute _validator;

        public static class ErrorMessage
        {
            public const string INVALID_EMAIL_FORMAT = "\"{0}\" is invalid email address";
        }

        public EmailValidator()
        {
            _validator = new EmailAddressAttribute();
        }

        public string Validate(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return CommonErrorMessage.EMAIL_REQUIRED;
            }

            if (!_validator.IsValid(email))
            {
                return string.Format(ErrorMessage.INVALID_EMAIL_FORMAT, email);
            }

            return null;
        }
    }
}
