using System;
using System.Collections.Generic;
using System.Text;
using Ziewaar.TtLog.LogFormats;

namespace Ziewaar.TtLog.Writers
{
    public interface ILogWriter : IDisposable
    {
        void Flush();
        void Write(DateTime adjustedTimestamp, LogEvent logEvent);
    }
}
