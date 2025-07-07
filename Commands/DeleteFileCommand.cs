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

        public DeleteFileCommand(string path)
        {
            _path = path;
        }

        public void Execute()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }
    }
}
