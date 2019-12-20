using System;
using Library.Objects.Services.Interfaces;

namespace Library.Objects.Services.Implementations
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
