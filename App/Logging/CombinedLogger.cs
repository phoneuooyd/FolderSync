using System;

namespace FolderSync.Logging
{
    public class CombinedLogger : ILogger
    {
        private readonly ILogger[] _loggers;
        public CombinedLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Log(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(message);
            }
        }

        public void Dispose()
        {
            foreach (var logger in _loggers)
            {
                logger.Dispose();
            }
        }
    }
}
