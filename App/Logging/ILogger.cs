﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.Logging
{
    public interface ILogger : IDisposable
    {
        void Log(string message);
    }
}
