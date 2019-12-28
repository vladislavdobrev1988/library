using Library.Objects.Helpers.Request;
using Library.Objects.Validation.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Library.Tests.ValidationTests
{
    [TestClass]
    public class PageValidatorTests
    {
        private readonly PageValidator _validator;

        public PageValidatorTests()
        {
            _validator = new PageValidator();
        }

        [TestMethod]
        public void Validate_NullRequest_ReturnsExpectedMessage()
        {
            var errorMessage = _validator.Validate(null);

            Assert.AreEqual(PageValidator.ErrorMessage.REQUIRED, errorMessage);
        }

        [TestMethod]
        public void Validate_PageZero_ReturnsExpectedMessage()
        {
            var request = new PageRequest
            {
                Page = 0,
                Size = 10
            };

            var errorMessage = _validator.Validate(request);

            Assert.AreEqual(PageValidator.ErrorMessage.PAGE, errorMessage);
        }

        [TestMethod]
        public void Validate_SizeZero_ReturnsExpectedMessage()
        {
            var request = new PageRequest
            {
                Page = 1,
                Size = 0
            };

            var errorMessage = _validator.Validate(request);

            Assert.AreEqual(PageValidator.ErrorMessage.Size, errorMessage);
        }

        [TestMethod]
        public void Validate_SizeAboveLimit_ReturnsExpectedMessage()
        {
            var request = new PageRequest
            {
                Page = 1,
                Size = 21
            };

            var errorMessage = _validator.Validate(request);

            Assert.AreEqual(PageValidator.ErrorMessage.Size, errorMessage);
        }

        [TestMethod]
        public void Validate_ValidRequest_ReturnsNull()
        {
            var request = new PageRequest
            {
                Page = 1,
                Size = 10
            };

            var errorMessage = _validator.Validate(request);

            Assert.IsNull(errorMessage);
        }
    }
}
