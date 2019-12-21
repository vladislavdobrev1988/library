using Library.Objects.Helpers.Constants;
using Library.Objects.Validation.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Library.Tests.ValidationTests
{
    [TestClass]
    public class EmailValidatorTests
    {
        private readonly EmailValidator _validator;
        
        public EmailValidatorTests()
        {
            _validator = new EmailValidator();
        }

        [TestMethod]
        public void Validate_NullEmail_ReturnsExpectedMessage()
        {
            var error = _validator.Validate(null);

            Assert.AreEqual(CommonErrorMessage.EMAIL_REQUIRED, error);
        }

        [TestMethod]
        public void Validate_WhiteSpaceEmail_ReturnsExpectedMessage()
        {
            var email = " ";

            var error = _validator.Validate(email);

            Assert.AreEqual(CommonErrorMessage.EMAIL_REQUIRED, error);
        }

        [TestMethod]
        public void Validate_InvalidEmail_ReturnsExpectedMessage()
        {
            var email = "a";

            var error = _validator.Validate(email);

            var expectedMessage = string.Format(EmailValidator.ErrorMessage.INVALID_EMAIL_FORMAT, email);

            Assert.AreEqual(expectedMessage, error);
        }

        [TestMethod]
        public void Validate_GoodEmail_ReturnsNull()
        {
            var email = "a@a.com";

            var error = _validator.Validate(email);

            Assert.IsNull(error);
        }
    }
}
