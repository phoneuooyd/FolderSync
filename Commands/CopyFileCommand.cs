using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.Commands
{
    public class CopyFileCommand : IFileCommand
    {
        private readonly string _source;
        private readonly string _destination;

        public CopyFileCommand(string source, string destination)
        {
            _source = source;
            _destination = destination;
        }

        public void Execute()
        {
            var directory = Path.GetDirectoryName(_destination);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
            File.Copy(_source, _destination, true);
        }
    }
}
