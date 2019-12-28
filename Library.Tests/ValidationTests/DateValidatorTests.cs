using Library.Objects.Helpers.Constants;
using Library.Objects.Validation.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Library.Tests.ValidationTests
{
    [TestClass]
    public class DateValidatorTests
    {
        private readonly DateValidator _validator;

        public DateValidatorTests()
        {
            _validator = new DateValidator();
        }

        [TestMethod]
        public void Validate_NullDate_ReturnsExpectedMessage()
        {
            var errorMessage = _validator.Validate(null);

            Assert.AreEqual(DateValidator.ErrorMessage.REQUIRED, errorMessage);
        }

        [TestMethod]
        public void Validate_WhiteSpaceDate_ReturnsExpectedMessage()
        {
            var date = " ";

            var errorMessage = _validator.Validate(date);

            Assert.AreEqual(DateValidator.ErrorMessage.REQUIRED, errorMessage);
        }

        [TestMethod]
        public void Validate_DateInUnexpectedFormat_ReturnsExpectedMessage()
        {
            var date = "28/12/2019";

            var errorMessage = _validator.Validate(date);

            var expectedMessage = string.Format(DateValidator.ErrorMessage.INVALID_FORMAT, date, DateFormat.ISO_8601);

            Assert.AreEqual(expectedMessage, errorMessage);
        }

        [TestMethod]
        public void Validate_ValidDate_ReturnsNull()
        {
            var date = "2019-12-28";

            var errorMessage = _validator.Validate(date);

            Assert.IsNull(errorMessage);
        }
    }
}
