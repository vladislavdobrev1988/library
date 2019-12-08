using Library.Objects.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Library.Tests.ValidationTests
{
    [TestClass]
    public class PasswordValidationTests
    {
        [TestMethod]
        public void Validate_NullPassword_ReturnsExpectedMessage()
        {
            var error = Password.Validate(null);

            Assert.AreEqual(Password.ErrorMessage.Required, error);
        }
        [TestMethod]
        public void Validate_WhiteSpacePassword_ReturnsExpectedMessage()
        {
            var password = " ";

            var error = Password.Validate(password);

            Assert.AreEqual(Password.ErrorMessage.Required, error);
        }

        [TestMethod]
        public void Validate_ShortPassword_ReturnsExpectedMessage()
        {
            var password = "aA1_";

            var error = Password.Validate(password);

            Assert.AreEqual(Password.ErrorMessage.MinCharacterCount, error);
        }

        [TestMethod]
        public void Validate_PasswordWithInvalidChar_ReturnsExpectedMessage()
        {
            var password = "aa aaaA1_";

            var error = Password.Validate(password);

            Assert.AreEqual(Password.ErrorMessage.ValidCharacters, error);
        }

        [TestMethod]
        public void Validate_PasswordWithNonLatinLetter_ReturnsExpectedMessage()
        {
            var password = "ЖааaaaA1_";

            var error = Password.Validate(password);

            Assert.AreEqual(Password.ErrorMessage.ValidCharacters, error);
        }

        [TestMethod]
        public void Validate_PasswordWithoutLowerCase_ReturnsExpectedMessage()
        {
            var password = "AAAAAA2$";

            var error = Password.Validate(password);

            Assert.AreEqual(Password.ErrorMessage.MinLowerLetterCount, error);
        }

        [TestMethod]
        public void Validate_PasswordWithoutUpperCase_ReturnsExpectedMessage()
        {
            var password = "bbbbbb3=";

            var error = Password.Validate(password);

            Assert.AreEqual(Password.ErrorMessage.MinUpperLetterCount, error);
        }

        [TestMethod]
        public void Validate_PasswordWithoutDigit_ReturnsExpectedMessage()
        {
            var password = "abcd_XYZ";

            var error = Password.Validate(password);

            Assert.AreEqual(Password.ErrorMessage.MinDigitCount, error);
        }

        [TestMethod]
        public void Validate_PasswordWithoutSpecialChar_ReturnsExpectedMessage()
        {
            var password = "hijkLMN0";

            var error = Password.Validate(password);

            Assert.AreEqual(Password.ErrorMessage.MinSpecialCharacterCount, error);
        }

        [TestMethod]
        public void Validate_GoodPassword_ReturnsNull()
        {
            var password = "1q2w3e$R";

            var error = Password.Validate(password);

            Assert.IsNull(error);
        }
    }
}
