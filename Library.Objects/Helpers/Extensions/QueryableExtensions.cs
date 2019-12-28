using System.Linq;
using Library.Objects.Helpers.Request;

namespace Library.Objects.Helpers.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> GetPage<T>(this IQueryable<T> source, PageRequest pageRequest)
        {
            var skip = (pageRequest.Page - 1) * pageRequest.Size;

            return source.Skip(skip).Take(pageRequest.Size);
        }
    }
}
