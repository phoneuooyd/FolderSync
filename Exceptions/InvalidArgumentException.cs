using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.Exceptions
{
    public class InvalidArgumentsException : Exception
    {
        public InvalidArgumentsException(string message) : base(message) { }
    }
}

