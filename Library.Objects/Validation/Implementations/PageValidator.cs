using Library.Objects.Helpers.Request;
using Library.Objects.Validation.Interfaces;

namespace Library.Objects.Validation.Implementations
{
    public class PageValidator : IPageValidator
    {
        private static class ErrorMessage
        {
            public const string REQUIRED = "Page request is required";
            public const string PAGE = "Page value must be positive integer";

            public static readonly string Size = string.Format("Size value must be integer between 1 and {0}", MAX_PAGE_SIZE);
        }

        private const int MAX_PAGE_SIZE = 20;

        public string Validate(PageRequest request)
        {
            if (request == null)
            {
                return ErrorMessage.REQUIRED;
            }

            if (request.Page < 1)
            {
                return ErrorMessage.PAGE;
            }

            if (request.Size < 1 || request.Size > MAX_PAGE_SIZE)
            {
                return ErrorMessage.Size;
            }

            return null;
        }
    }
}
