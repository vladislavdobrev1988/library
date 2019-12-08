using System.Net;
using Library.Objects.Exceptions;

namespace Library.Objects.Models.Base
{
    public abstract class BaseModel
    {
        protected static void ThrowHttpConflict(string message)
        {
            throw new HttpResponseException(HttpStatusCode.Conflict, message);
        }

        protected static void ThrowHttpBadRequest(string message)
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, message);
        }
    }
}
