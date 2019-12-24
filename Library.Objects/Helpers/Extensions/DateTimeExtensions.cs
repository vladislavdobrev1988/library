using System;
using Library.Objects.Helpers.Constants;

namespace Library.Objects.Helpers.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToIsoDateString(this DateTime dateTime)
        {
            return dateTime.ToString(DateFormat.ISO_8601);
        }
    }
}
