using System;

namespace Library.OnStartup
{
    public class Logging
    {
        private const string FORMAT = "{0:yyyy-MM}.txt";

        public static string GetLogFileName()
        {
            return string.Format(FORMAT, DateTime.UtcNow);
        }
    }
}
