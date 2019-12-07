using System;
using System.IO;
using System.Threading.Tasks;
using Library.Objects.Services.Interfaces;

namespace Library.Objects.Services.Implementations
{
    public class ExceptionLogger : IExceptionLogger
    {
        private readonly string _path;
        private readonly Func<string> _getFileName;

        private const string TIMESTAMP_FORMAT = "u";

        public ExceptionLogger(string path, Func<string> getFileName)
        {
            _path = path;
            _getFileName = getFileName;

            Directory.CreateDirectory(_path);
        }

        public async Task LogAsync(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var path = Path.Combine(_path, _getFileName());
            var text = CreateLogEntry(exception);

            await File.AppendAllTextAsync(path, text);
        }

        private string CreateLogEntry(Exception exception)
        {
            return string.Join(Environment.NewLine, DateTime.UtcNow.ToString(TIMESTAMP_FORMAT), exception.ToString(), Environment.NewLine);
        }
    }
}
