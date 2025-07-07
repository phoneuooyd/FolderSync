using FolderSync.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.Commands
{
    public class DeleteFileCommand : IFileCommand
    {
        private readonly string _path;
        private readonly ILogger _logger;

        public DeleteFileCommand(string path, ILogger logger)
        {
            _path = path;
            _logger = logger;
        }

        public void Execute()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
                _logger.Log($"Deleted {_path}");
            }
        }
    }
}
