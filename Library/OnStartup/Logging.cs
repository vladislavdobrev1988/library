using System;

namespace Library.OnStartup
{
    public class Logging
    {
        private const string DATE_FORMAT = "yyyy-MM";
        private const string LOG_FILE_EXTENSION = ".txt";

        public static string GetLogFileName()
        {
            var name = DateTime.UtcNow.ToString(DATE_FORMAT);
            return string.Concat(name, LOG_FILE_EXTENSION);
        }
    }
}
