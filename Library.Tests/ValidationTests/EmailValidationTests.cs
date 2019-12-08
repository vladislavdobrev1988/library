using Library.Objects.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Library.Tests.ValidationTests
{
    [TestClass]
    public class EmailValidationTests
    {
        [TestMethod]
        public void Validate_NullEmail_ReturnsExpectedMessage()
        {
            var error = Email.Validate(null);

            Assert.AreEqual(Email.ErrorMessage.REQUIRED, error);
        }

        [TestMethod]
        public void Validate_WhiteSpaceEmail_ReturnsExpectedMessage()
        {
            var email = " ";

            var error = Email.Validate(email);

            Assert.AreEqual(Email.ErrorMessage.REQUIRED, error);
        }

        [TestMethod]
        public void Validate_InvalidEmail_ReturnsExpectedMessage()
        {
            var email = "a";

            var error = Email.Validate(email);

            var expectedMessage = string.Format(Email.ErrorMessage.INVALID_EMAIL_FORMAT, email);

            Assert.AreEqual(expectedMessage, error);
        }

        [TestMethod]
        public void Validate_GoodEmail_ReturnsNull()
        {
            var email = "a@a.com";

            var error = Email.Validate(email);

            Assert.IsNull(error);
        }
    }
}
