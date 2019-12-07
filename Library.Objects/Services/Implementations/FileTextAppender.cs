using System;
using System.IO;
using System.Threading.Tasks;
using Library.Objects.Services.Interfaces;

namespace Library.Objects.Services.Implementations
{
    public class FileTextAppender : ITextAppender
    {
        private readonly string _path;
        private readonly Func<string> _getFileName;

        public FileTextAppender(string path, Func<string> getFileName)
        {
            _path = path;
            _getFileName = getFileName;

            Directory.CreateDirectory(_path);
        }

        public async Task AppendTextAsync(string text)
        {
            await File.AppendAllTextAsync(Path.Combine(_path, _getFileName()), text);
        }
    }
}
