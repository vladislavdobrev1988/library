using System;
using System.Linq;
using Library.Objects.Helpers.Extensions;
using Library.Objects.Helpers.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Library.Tests.ExtensionTests
{
    [TestClass]
    public class QueryableExtensionTests
    {
        private const char A = 'a';
        private const char B = 'b';
        private const char C = 'c';
        private const char D = 'd';

        private IQueryable<char> _source;

        public QueryableExtensionTests()
        {
            _source = new[] { A, B, C, D }.AsQueryable();
        }

        [TestMethod]
        public void GetPage_NullSource_ThrowsException()
        {
            IQueryable<char> source = null;
            var pageRequest = new PageRequest
            {
                Page = 1,
                Size = 2
            };

            var ex = Assert.ThrowsException<ArgumentNullException>(() => source.GetPage(pageRequest));

            Assert.AreEqual(nameof(source), ex.ParamName);
        }

        [TestMethod]
        public void GetPage_NullRequest_ThrowsException()
        {
            PageRequest pageRequest = null;

            var ex = Assert.ThrowsException<ArgumentNullException>(() => _source.GetPage(pageRequest));

            Assert.AreEqual(nameof(pageRequest), ex.ParamName);
        }

        [TestMethod]
        public void GetPage_ValidRequest1_ReturnsExpectedResult()
        {
            var pageRequest = new PageRequest
            {
                Page = 1,
                Size = 2
            };

            var result = _source.GetPage(pageRequest).ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(pageRequest.Size, result.Length);
            Assert.AreEqual(A, result[0]);
            Assert.AreEqual(B, result[1]);
        }

        [TestMethod]
        public void GetPage_ValidRequest2_ReturnsExpectedResult()
        {
            var pageRequest = new PageRequest
            {
                Page = 2,
                Size = 2
            };

            var result = _source.GetPage(pageRequest).ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(pageRequest.Size, result.Length);
            Assert.AreEqual(C, result[0]);
            Assert.AreEqual(D, result[1]);
        }

        [TestMethod]
        public void GetPage_ValidRequest3_ReturnsExpectedResult()
        {
            var pageRequest = new PageRequest
            {
                Page = 2,
                Size = 3
            };

            var result = _source.GetPage(pageRequest).ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(D, result.Single());
        }
    }
}
