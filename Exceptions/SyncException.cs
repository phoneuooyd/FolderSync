using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.Exceptions
{
    public class SyncException : Exception
    {
        public SyncException(string message, Exception inner) : base(message, inner) { }
    }
}
