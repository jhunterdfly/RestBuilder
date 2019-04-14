using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RestBuilder.Service
{
    public interface IRbLoggingService
    {
        void Log(string message, IDictionary<string, string> properties = null);
        void Log(Exception exc);
    }
}
