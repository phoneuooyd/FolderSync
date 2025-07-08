using System;
using System.IO;
using FolderSync.Exceptions;

namespace FolderSync.Logging
{
    public class FileLogger : ILogger
    {
        private readonly StreamWriter _writer;
        public string FilePath { get; }

        public FileLogger(string filePath)
        {
            FilePath = filePath;
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }
                _writer = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    AutoFlush = true
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new SyncException($"Access denied to log file '{filePath}'", ex);
            }
        }

        public void Log(string message)
        {
            _writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
