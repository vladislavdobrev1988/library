using Library.Objects.Helpers.Constants;
using Library.Objects.Validation.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Library.Tests.ValidationTests
{
    [TestClass]
    public class PasswordValidatorTests
    {
        private PasswordValidator _validator;

        [TestInitialize]
        public void Init()
        {
            _validator = new PasswordValidator();
        }

        [TestMethod]
        public void Validate_NullPassword_ReturnsExpectedMessage()
        {
            var error = _validator.Validate(null);

            Assert.AreEqual(CommonErrorMessage.PASSWORD_REQUIRED, error);
        }
        [TestMethod]
        public void Validate_WhiteSpacePassword_ReturnsExpectedMessage()
        {
            var password = " ";

            var error = _validator.Validate(password);

            Assert.AreEqual(CommonErrorMessage.PASSWORD_REQUIRED, error);
        }

        [TestMethod]
        public void Validate_ShortPassword_ReturnsExpectedMessage()
        {
            var password = "aA1_";

            var error = _validator.Validate(password);

            Assert.AreEqual(PasswordValidator.ErrorMessage.MinCharacterCount, error);
        }

        [TestMethod]
        public void Validate_PasswordWithInvalidChar_ReturnsExpectedMessage()
        {
            var password = "aa aaaA1_";

            var error = _validator.Validate(password);

            Assert.AreEqual(PasswordValidator.ErrorMessage.ValidCharacters, error);
        }

        [TestMethod]
        public void Validate_PasswordWithNonLatinLetter_ReturnsExpectedMessage()
        {
            var password = "ЖааaaaA1_";

            var error = _validator.Validate(password);

            Assert.AreEqual(PasswordValidator.ErrorMessage.ValidCharacters, error);
        }

        [TestMethod]
        public void Validate_PasswordWithoutLowerCase_ReturnsExpectedMessage()
        {
            var password = "AAAAAA2$";

            var error = _validator.Validate(password);

            Assert.AreEqual(PasswordValidator.ErrorMessage.MinLowerLetterCount, error);
        }

        [TestMethod]
        public void Validate_PasswordWithoutUpperCase_ReturnsExpectedMessage()
        {
            var password = "bbbbbb3=";

            var error = _validator.Validate(password);

            Assert.AreEqual(PasswordValidator.ErrorMessage.MinUpperLetterCount, error);
        }

        [TestMethod]
        public void Validate_PasswordWithoutDigit_ReturnsExpectedMessage()
        {
            var password = "abcd_XYZ";

            var error = _validator.Validate(password);

            Assert.AreEqual(PasswordValidator.ErrorMessage.MinDigitCount, error);
        }

        [TestMethod]
        public void Validate_PasswordWithoutSpecialChar_ReturnsExpectedMessage()
        {
            var password = "hijkLMN0";

            var error = _validator.Validate(password);

            Assert.AreEqual(PasswordValidator.ErrorMessage.MinSpecialCharacterCount, error);
        }

        [TestMethod]
        public void Validate_GoodPassword_ReturnsNull()
        {
            var password = "1q2w3e$R";

            var error = _validator.Validate(password);

            Assert.IsNull(error);
        }
    }
}
