using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.Comparers
{
    public interface IFileComparer
    {
        bool FilesAreDifferent(string sourceFile, string replicaFile);
    }
}
