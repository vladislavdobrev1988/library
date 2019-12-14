using Library.Objects.Exceptions;
using Library.Objects.Helpers.Constants;

namespace Library.Objects.Models.Base
{
    public abstract class BaseModel
    {
        protected static void ThrowHttpConflict(string message)
        {
            throw new HttpResponseException(HttpStatusCode.CONFLICT, message);
        }

        protected static void ThrowHttpBadRequest(string message)
        {
            throw new HttpResponseException(HttpStatusCode.BAD_REQUEST, message);
        }

        protected static void ThrowHttpUnauthorized(string message)
        {
            throw new HttpResponseException(HttpStatusCode.UNAUTHORIZED, message);
        }
    }
}
