using System;
using System.Threading.Tasks;
using Library.Objects.Services.Interfaces;

namespace Library.Objects.Services.Implementations
{
    public class ExceptionLogger : IExceptionLogger
    {
        private readonly ITextAppender _appender;

        private const string TIMESTAMP_FORMAT = "u";

        public ExceptionLogger(ITextAppender appender)
        {
            _appender = appender;
        }

        public async Task LogAsync(Exception exception)
        {
            await _appender.AppendTextAsync(CreateLogEntry(exception));
        }

        private string CreateLogEntry(Exception exception)
        {
            return string.Join(Environment.NewLine, DateTime.UtcNow.ToString(TIMESTAMP_FORMAT), exception.ToString(), Environment.NewLine);
        }
    }
}
