using Library.Objects.Exceptions;
using Library.Objects.Helpers.Constants;

namespace Library.Objects.Helpers.Common
{
    public static class ThrowHttp
    {
        public static void Conflict(string message)
        {
            throw new HttpResponseException(HttpStatusCode.CONFLICT, message);
        }

        public static void BadRequest(string message)
        {
            throw new HttpResponseException(HttpStatusCode.BAD_REQUEST, message);
        }

        public static void Unauthorized(string message)
        {
            throw new HttpResponseException(HttpStatusCode.UNAUTHORIZED, message);
        }

        public static void NotFound(string message)
        {
            throw new HttpResponseException(HttpStatusCode.NOT_FOUND, message);
        }
    }
}
