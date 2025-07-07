
using FolderSync.Logging;

namespace FolderSync.Commands
{
    public class CopyFileCommand : IFileCommand
    {
        private readonly string _source;
        private readonly string _destination;
        private readonly ILogger _logger;

        public CopyFileCommand(string source, string destination, ILogger logger)
        {
            _source = source;
            _destination = destination;
            _logger = logger;
        }

        public void Execute()
        {
            var directory = Path.GetDirectoryName(_destination);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
            File.Copy(_source, _destination, true);
            _logger.Log($"Copied {_source} to {_destination}");
        }
    }
}
